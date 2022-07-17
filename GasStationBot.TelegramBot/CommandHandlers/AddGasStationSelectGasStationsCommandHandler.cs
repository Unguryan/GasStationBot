using GasStationBot.Application.Services;
using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.TelegramBot.Commands;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class AddGasStationSelectGasStationsCommandHandler : BaseTelegramCommandHandlerWithContext<AddGasStationSelectGasStationsCommand>
    {

        private readonly IEnumerable<IGasStationsService> _gasStationsServices;

        private readonly ITelegramUserStateService _userStateService;

        public AddGasStationSelectGasStationsCommandHandler(ITelegramBotClient botClient,
                                                            IEnumerable<IGasStationsService> gasStationsServices,
                                                            ITelegramUserStateService userStateService) : base(botClient)
        {
            _gasStationsServices = gasStationsServices;
            _userStateService = userStateService;
        }

        protected override Task<string> Message => GetCustomMessage();

        protected override Task<IReplyMarkup> Keyboard => GetCustomKeyboard();

        protected override async Task<UserState> HandleCommand()
        {
            if (CheckBackCommand())
            {
                return UserState.None;
            }

            var stations = await GetGasStationsBySelectedCity();
            if (!stations.Any())
            {
                await SendMessage(Command.UserId, "❌Доступних АЗС не знайдено або всі АЗС с цього міста вже додані.❌");
                return UserState.None;
            }

            var verifyResult = int.TryParse(Command.UserMessage, out int gasStationId);

            if (verifyResult)
            {
                var res = await CheckGasStationId(gasStationId);
                if (res.Item1)
                {
                    var tempData = _userStateService.GetUserTempData(Command.UserId);
                    tempData.SelectedGasStation = res.Item2;
                    tempData.SelectedFuels = new List<Fuel>();
                    _userStateService.SetUserTempData(Command.UserId, tempData);
                    return Command.NextState!.Value;
                }
            }

            await SendMessage(Command.UserId, "❌АЗС з таким ІД не знайдено, спробуйте ще.❌");

            //await SendMessage(Command.UserId, await Message, await Keyboard);
            return Command.UserState;
        }

        private async Task<Tuple<bool, GasStation>> CheckGasStationId(int gasStationId)
        {
            var gasStations = await GetGasStationsBySelectedCity();

            if (gasStationId <= 0 || gasStationId > gasStations.Count)
            {
                return new Tuple<bool, GasStation>(false, null);
            }

            return new Tuple<bool, GasStation>(true, gasStations[gasStationId - 1]); ;
        }

        private async Task<string> GetCustomMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<b>Введіть ІД АЗС ✅⛽️</b>\n(або оберіть на клавіатурі): ");

            var gasStations = await GetGasStationsBySelectedCity();

            if (!gasStations.Any())
            {
                return "❌Доступних АЗС не знайдено або всі АЗС с цього міста вже додані.❌";
            }

            for (int i = 0; i < gasStations.Count; i++)
            {
                sb.AppendLine($"<b>✅ІД: {i + 1}</b>\n Адреса: <b>{gasStations[i].Address}</b>\n");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private async Task<IReplyMarkup> GetCustomKeyboard()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);

            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "До головної" });

            var gasStations = await GetGasStationsBySelectedCity();

            var id = 1;
            for (int i = 0; i < gasStations.Count(); i += 4)
            {
                var subList = gasStations.GetRange(i, Math.Min(4, gasStations.Count() - i));
                var subKeyboard = new KeyboardButton[subList.Count];
                for (int j = 0; j < subList.Count; j++)
                {
                    subKeyboard[j] = new KeyboardButton(id.ToString());
                    id++;
                }

                keyboard.Add(subKeyboard);
            }


            return new ReplyKeyboardMarkup(keyboard);
        }

        //TODO: Move this method to service
        //TODO: Remove existing user GasStations, see avaliable GasStationForUser
        private async Task<List<GasStation>> GetGasStationsBySelectedCity()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            var gsService = _gasStationsServices.SingleOrDefault(u => u.GasStationName == tempData.ProviderName);
            var user = await UserService.GetUserById(Command.UserId);
            if (gsService == null || user == null)
            {
                return new List<GasStation>();
            }

            var gasStations = await gsService.GetGasStationsWithoutAdditionalData();
            if (!user.GasStations.Any())
            {
                return gasStations.ToList();
            }

            var list = new List<GasStation>();
            foreach (var gasStation in gasStations.Where(gs => gs.City == tempData.City))
            {
                if (!user.GasStations.Any(gs => gs.City == gasStation.City
                                             && gs.Address == gasStation.Address
                                             && gs.Provider == gasStation.Provider))
                {
                    list.Add(gasStation);
                }
            }

            return list;
        }
    }
}
