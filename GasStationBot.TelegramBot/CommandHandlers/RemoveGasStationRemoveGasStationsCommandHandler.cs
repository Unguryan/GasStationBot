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
    public class RemoveGasStationRemoveGasStationsCommandHandler : BaseTelegramCommandHandler<RemoveGasStationRemoveGasStationsCommand>
    {

        private readonly ITelegramUserStateService _userStateService;

        private readonly IUserService _userService;

        public RemoveGasStationRemoveGasStationsCommandHandler(ITelegramBotClient botClient,
                                                               ITelegramUserStateService userStateService,
                                                               IUserService userService) : base(botClient)
        {
            _userStateService = userStateService;
            _userService = userService;
        }

        protected override string Message => GetCustomMessage();

        protected override IReplyMarkup Keyboard => GetCustomKeyboard();

        protected override async Task<UserState> HandleCommand()
        {
            if (CheckBackCommand())
            {
                return UserState.None;
            }

            var tempData = _userStateService.GetUserTempData(Command.UserId);
            if (tempData == null ||
                tempData.SelectedGasStation == null)
            {
                await SendMessage(Command.UserId, "Помилка, АЗС не було обрано.");
                return UserState.RemoveGasStation_ListGasStations;
            }

            if (Command.UserMessage == "Видалити АЗС" && CheckGasStation(out GasStation gasStation))
            {
                var res = await _userService.RemoveGasStationFromUser(Command.UserId, gasStation);
                _userStateService.ClearTempData(Command.UserId);
                var message = res ? "АЗС видалена" : "Помилка. АЗС не видалена. Повертаю до головного меню.";
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

            gasStation.Fuels.AddRange(tempData.SelectedGasStation.Fuels);
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

            sb.AppendLine("Якщо усе ок, обирайте (на клавіатурі) \"Видалити АЗС\"\n");

            return sb.ToString();
        }

        private IReplyMarkup GetCustomKeyboard()
        {
            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "Видалити АЗС" });
            keyboard.Add(new KeyboardButton[] { "До головної" });

            return new ReplyKeyboardMarkup(keyboard);
        }
    }
}
