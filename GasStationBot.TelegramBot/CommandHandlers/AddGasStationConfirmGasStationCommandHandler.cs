using GasStationBot.Application.Services;
using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Extensions;
using GasStationBot.TelegramBot.Commands;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class AddGasStationConfirmGasStationCommandHandler : BaseTelegramCommandHandler<AddGasStationConfirmGasStationCommand>
    {

        private readonly IUserService _userService;

        private readonly ITelegramUserStateService _userStateService;

        public AddGasStationConfirmGasStationCommandHandler(ITelegramBotClient botClient,
                                                            IUserService userService,
                                                            ITelegramUserStateService userStateService) : base(botClient)
        {
            _userService = userService;
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

            if (Command.UserMessage == "Змінити Паливо")
            {
                return UserState.AddGasStation_SelectFuel;
            }

            var tempData = _userStateService.GetUserTempData(Command.UserId);
            if (tempData == null ||
                tempData.SelectedFuels == null ||
               !tempData.SelectedFuels.Any() ||
                tempData.SelectedGasStation == null)
            {
                await SendMessage(Command.UserId, "Помилка, АЗС не була сбережена, повертаю до головного меню. Якщо ви це бачите, напишить автору цього бота. Він сможе допомогти.");
                return UserState.None;
            }

            if (Command.UserMessage == "Додати АЗС" && CheckGasStation(out GasStation gasStation))
            {
                await _userService.AddGasStationToUser(Command.UserId, gasStation);
                _userStateService.ClearTempData(Command.UserId);
                return Command.NextState!.Value;
            }

            await SendMessage(Command.UserId, "Помилка, такої команди нема, спробуйте ще.");
            await SendMessage(Command.UserId, Message, Keyboard);
            return Command.UserState;
        }

        private bool CheckGasStation(out GasStation gasStation)
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);

            if (tempData == null ||
                tempData.SelectedFuels == null ||
               !tempData.SelectedFuels.Any() ||
                tempData.SelectedGasStation == null)
            {
                gasStation = null;
                return false;
            }

            gasStation = new GasStation()
            {
                Provider = tempData.SelectedGasStation.Provider,
                Address = tempData.SelectedGasStation.Address,
                City = tempData.SelectedGasStation.City,
                Fuels = new List<Fuel>()
            };

            gasStation.Fuels.AddRange(tempData.SelectedFuels);
            return true;
        }

        private string GetCustomMessage()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            var sb = new StringBuilder();

            sb.AppendLine("Обране АЗС:");
            sb.AppendLine($"Власник АЗС: {tempData.SelectedGasStation.Provider}");
            sb.AppendLine($"Місто: {tempData.SelectedGasStation.City}");
            sb.AppendLine($"Адреса: {tempData.SelectedGasStation.Address}");

            sb.AppendLine("Обране паливо:");
            foreach (var fuel in tempData.SelectedFuels)
            {
                sb.Append($"{fuel.FuelType.GetDescription()}  ");
            }

            sb.AppendLine("Якщо усе ок, обирайте (на клавіатурі) \"Додати АЗС\"\n");

            return sb.ToString();
        }

        private IReplyMarkup GetCustomKeyboard()
        {
            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "Додати АЗС" });
            keyboard.Add(new KeyboardButton[] { "Змінити Паливо" });
            keyboard.Add(new KeyboardButton[] { "До головної" });

            return new ReplyKeyboardMarkup(keyboard);
        }
    }
}
