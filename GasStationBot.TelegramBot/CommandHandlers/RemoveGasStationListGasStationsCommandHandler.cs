using GasStationBot.Application.Services;
using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Entities.Temp;
using GasStationBot.Domain.Extensions;
using GasStationBot.TelegramBot.Commands;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class RemoveGasStationListGasStationsCommandHandler : BaseTelegramCommandHandler<RemoveGasStationListGasStationsCommand>
    {

        private readonly ITelegramUserStateService _userStateService;
        
        private readonly IUserService _userService;

        public RemoveGasStationListGasStationsCommandHandler(ITelegramBotClient botClient,
                                                             ITelegramUserStateService userStateService,
                                                             IUserService userService) : base(botClient)
        {
            _userStateService = userStateService;
            _userService = userService;
        }

        protected override Task<string> Message => GetCustomMessage();

        protected override Task<IReplyMarkup> Keyboard => GetCustomKeyboard();

        protected override async Task<UserState> HandleCommand()
        {
            if (CheckBackCommand())
            {
                return UserState.None;
            }

            var user = await _userService.GetUserById(Command.UserId);
            
            if(user.GasStations == null || !user.GasStations.Any())
            {
                await SendMessage(Command.UserId, "Ви ще не додали жодної АЗС.");
                return UserState.None;
            }

            if(int.TryParse(Command.UserMessage, out int gasStationId))
            {
                var res = await CheckID(gasStationId);
                if (res.Item1)
                {
                    _userStateService.SetUserTempData(Command.UserId, new TempUserDataModel() { SelectedGasStation = res.Item2 });
                    return Command.NextState!.Value;
                }
            }

            await SendMessage(Command.UserId, "АЗС з таким ІД не знайдено, спробуйте ще.");
            await SendMessage(Command.UserId, await Message, await Keyboard);
            return Command.UserState;
        }

        private async Task<Tuple<bool, GasStation>> CheckID(int gasStationId)
        {
            var user = await _userService.GetUserById(Command.UserId);

            if (gasStationId <= 0 || gasStationId > user.GasStations.Count)
            {
                return new Tuple<bool, GasStation>(false, null);
            }

            var gasStation = user.GasStations[gasStationId - 1];
            return new Tuple<bool, GasStation>(true, gasStation);
        }

        private async Task<string> GetCustomMessage()
        {
            //TODO: Fix it
            var user = await _userService.GetUserById(Command.UserId);

            if (user.GasStations == null ||
               !user.GasStations.Any())
            {
                return "У вас ще нема підписок";
            }

            var sb = new StringBuilder();
            sb.AppendLine("Ваші підписки: ");
            sb.AppendLine();
            var counter = 1;
            foreach (var gs in user.GasStations)
            {
                sb.AppendLine($"ІД: {counter++}");
                sb.AppendLine($"Власник АЗС: {gs.Provider}");
                sb.AppendLine($"Місто: {gs.City}");
                sb.AppendLine($"Адреса: {gs.Address}");

                sb.AppendLine($"Обране Пальне (яке відстежується): ");
                foreach (var fuel in gs.Fuels)
                {
                    sb.Append($"{fuel.FuelType.GetDescription()}  ");
                }

                sb.AppendLine();
            }

            sb.AppendLine($"Оберіть ІД АЗС (на клавіатурі), Яке потрібно видалити:");

            return sb.ToString();
        }

        private async Task<IReplyMarkup> GetCustomKeyboard()
        {
            var user = await _userService.GetUserById(Command.UserId);
            var gasStations = user.GasStations.Select(g => g.Address).ToList();

            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "До головної" });

            var counter = 1;
            for (int i = 0; i < gasStations.Count(); i += 4)
            {
                var subKeyboard = new KeyboardButton[4];
                var subList = gasStations.GetRange(i, Math.Min(4, gasStations.Count() - i));
                for (int j = 0; j < subList.Count; j++)
                {
                    subKeyboard[j] = new KeyboardButton((counter++).ToString());
                }

                keyboard.Add(subKeyboard);
            }

            return new ReplyKeyboardMarkup(keyboard);
        }
    }
}
