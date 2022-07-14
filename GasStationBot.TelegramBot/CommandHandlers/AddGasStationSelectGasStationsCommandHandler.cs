using GasStationBot.Application.Services;
using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.TelegramBot.Commands;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class AddGasStationSelectGasStationsCommandHandler : BaseTelegramCommandHandler<AddGasStationSelectGasStationsCommand>
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

        protected override string Message => GetCustomMessage();

        protected override IReplyMarkup Keyboard => GetCustomKeyboard();

        protected override async Task<UserState> HandleCommand()
        {
            if (CheckBackCommand())
            {
                return UserState.None;
            }

            if (!GetGasStationsBySelectedCity().Any())
            {
                await SendMessage(Command.UserId, "Доступних АЗС не знайдено або всі АЗС с цього міста вже додані.");
                return UserState.None;
            }

            if (int.TryParse(Command.UserMessage, out int gasStationId) &&
                CheckGasStationId(gasStationId, out GasStation selectedGasStation))
            {
                var tempData = _userStateService.GetUserTempData(Command.UserId);
                tempData.SelectedGasStation = selectedGasStation;
                _userStateService.SetUserTempData(Command.UserId, tempData);
                return Command.NextState!.Value;
            }

            await SendMessage(Command.UserId, "АЗС з таким ІД не знайдено, спробуйте ще.");

            await SendMessage(Command.UserId, Message, Keyboard);
            return Command.UserState;
        }

        private bool CheckGasStationId(int gasStationId, out GasStation selectedGasStation)
        {
            var gasStations = GetGasStationsBySelectedCity();

            if (gasStationId <= 0 || gasStationId > gasStations.Count)
            {
                selectedGasStation = null;
                return false;
            }

            selectedGasStation = gasStations[gasStationId - 1];
            return true;
        }

        private string GetCustomMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Введіть ІД АЗС (або оберіть на клавіатурі): ");

            var gasStations = GetGasStationsBySelectedCity();

            for (int i = 0; i < gasStations.Count; i++)
            {
                sb.AppendLine($"ІД: {i + 1}. Адреса: {gasStations[i].Address}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private IReplyMarkup GetCustomKeyboard()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);

            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "До головної" });

            var gasStations = GetGasStationsBySelectedCity();

            var id = 1;
            for (int i = 0; i < gasStations.Count(); i += 4)
            {
                var subKeyboard = new KeyboardButton[4];
                var subList = gasStations.GetRange(i, Math.Min(4, gasStations.Count() - i));
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
        private List<GasStation> GetGasStationsBySelectedCity()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            var gsService = _gasStationsServices.SingleOrDefault(u => u.GasStationName == tempData.ProviderName);
            if (gsService == null)
            {
                return new List<GasStation>();
            }

            var gasStationsTask = gsService.GetGasStationsWithoutAdditionalData();
            gasStationsTask.Wait();
            return gasStationsTask.Result.Where(gs => gs.City == tempData.City).ToList();
        }
    }
}
