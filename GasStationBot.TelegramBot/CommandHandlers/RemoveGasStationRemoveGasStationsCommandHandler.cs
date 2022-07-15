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
    public class RemoveGasStationRemoveGasStationsCommandHandler : BaseTelegramCommandHandlerWithContext<RemoveGasStationRemoveGasStationsCommand>
    {

        private readonly ITelegramUserStateService _userStateService;

        public RemoveGasStationRemoveGasStationsCommandHandler(ITelegramBotClient botClient,
                                                               ITelegramUserStateService userStateService) : base(botClient)
        {
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

            var tempData = _userStateService.GetUserTempData(Command.UserId);
            if (tempData == null ||
                tempData.SelectedGasStation == null)
            {
                await SendMessage(Command.UserId, "Помилка, АЗС не було обрано.");
                return UserState.RemoveGasStation_ListGasStations;
            }

            var result = await CheckGasStation();
            if (Command.UserMessage == "Видалити АЗС" && result.Item1)
            {
                var res = await UserService.RemoveGasStationFromUser(Command.UserId, result.Item2);
                _userStateService.ClearTempData(Command.UserId);
                var message = res ? "АЗС видалена" : "Помилка. АЗС не видалена. Повертаю до головного меню.";
                return Command.NextState!.Value;
            }

            await SendMessage(Command.UserId, "Помилка, такої команди нема, спробуйте ще.");
            //await SendMessage(Command.UserId, await Message, await Keyboard);
            return Command.UserState;
        }

        private async Task<Tuple<bool, GasStation>> CheckGasStation()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);

            if (tempData == null ||
                tempData.SelectedGasStation == null)
            {
                return new Tuple<bool, GasStation>(false, null);
            }

            var gasStation = new GasStation()
            {
                Provider = tempData.SelectedGasStation.Provider,
                Address = tempData.SelectedGasStation.Address,
                City = tempData.SelectedGasStation.City,
                Fuels = new List<Fuel>()
            };

            gasStation.Fuels.AddRange(tempData.SelectedGasStation.Fuels);
            return new Tuple<bool, GasStation>(true, gasStation);
        }

        private Task<string> GetCustomMessage()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            var sb = new StringBuilder();

            sb.AppendLine("Обране АЗС:");
            sb.AppendLine($"Власник АЗС: {tempData.SelectedGasStation.Provider}");
            sb.AppendLine($"Місто: {tempData.SelectedGasStation.City}");
            sb.AppendLine($"Адреса: {tempData.SelectedGasStation.Address}");

            sb.AppendLine("Обране паливо:");
            foreach (var fuel in tempData.SelectedGasStation.Fuels)
            {
                sb.Append($"{fuel.FuelType.GetDescription()}  ");
            }

            sb.AppendLine();
            sb.AppendLine("Якщо усе ок, обирайте (на клавіатурі) \"Видалити АЗС\"\n");

            return Task.FromResult(sb.ToString());
        }

        private async Task<IReplyMarkup> GetCustomKeyboard()
        {
            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "Видалити АЗС" });
            keyboard.Add(new KeyboardButton[] { "До головної" });

            return new ReplyKeyboardMarkup(keyboard);
        }
    }
}
