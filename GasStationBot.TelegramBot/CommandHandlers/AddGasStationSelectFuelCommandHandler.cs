using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Extensions;
using GasStationBot.TelegramBot.Commands;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class AddGasStationSelectFuelCommandHandler : BaseTelegramCommandHandler<AddGasStationSelectFuelCommand>
    {

        private readonly ITelegramUserStateService _userStateService;

        public AddGasStationSelectFuelCommandHandler(ITelegramBotClient botClient,
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
            if (tempData.SelectedGasStation == null)
            {
                await SendMessage(Command.UserId, "❌Помилка! АЗС не знайдена, спробуйте обрати знов.❌");
                return UserState.AddGasStation_SelectGasStations;
            }

            if (Command.UserMessage == "✅Підтвердити")
            {
                if(tempData.SelectedFuels != null && tempData.SelectedFuels.Any())
                {
                    return Command.NextState!.Value;
                }
                else
                {
                    await SendMessage(Command.UserId, "❌Помилка! Паливо не додано.❌");
                    return Command.UserState;
                }
            }

            if (Command.UserMessage == "⬅️Скинути обране пальне")
            {
                if (tempData.SelectedFuels != null && tempData.SelectedFuels.Any())
                {
                    ClearSelectedFuel();
                    return Command.UserState;
                }
                else
                {
                    await SendMessage(Command.UserId, "❌Помилка! Паливо не додано.❌");
                    return Command.UserState;
                }
                
            }

            if (CheckNewFuel(out Fuel fuel))
            {
                if(tempData.SelectedFuels == null)
                {
                    tempData.SelectedFuels = new List<Fuel>();
                }

                tempData.SelectedFuels.Add(fuel);
                _userStateService.SetUserTempData(Command.UserId, tempData);
                return Command.UserState;
            }

            await SendMessage(Command.UserId, "❌Пальне не знайдено або вже додано, спробуйте ще.❌");
            //await SendMessage(Command.UserId, await Message, await Keyboard);
            return Command.UserState;
        }

        private void ClearSelectedFuel()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            tempData.SelectedFuels = new List<Fuel>();
            _userStateService.SetUserTempData(Command.UserId, tempData);
        }

        private bool CheckNewFuel(out Fuel fuel)
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            var selectedFuel = tempData.SelectedGasStation.Fuels.SingleOrDefault(f => f.FuelType.GetDescription() == Command.UserMessage);
            if(selectedFuel == null)
            {
                fuel = null;
                return false;
            }

            fuel = selectedFuel;

            if (tempData.SelectedFuels == null)
            {
                return true;
            }

            return !tempData.SelectedFuels.Any(f => f.FuelType == selectedFuel.FuelType);
        }

        private async Task<string> GetCustomMessage()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            var sb = new StringBuilder();
            sb.AppendLine("<b>Обрана АЗС ⛽️</b>\n");
            sb.AppendLine($"Адреса: <b>{tempData.SelectedGasStation.Address}</b>\n");

            sb.AppendLine($"<b>Доступне пальне ✅:</b>");
            foreach (var fuel in tempData.SelectedGasStation!.Fuels)
            {
                if (!tempData.SelectedFuels.Any(f => f.FuelType == fuel.FuelType))
                {
                    sb.AppendLine($"📍{fuel.FuelType.GetDescription()}  ");
                    //availableFuel.Add(fuel);
                }
            }

            sb.AppendLine();
            sb.AppendLine($"<b>Обране пальне:</b>");
            foreach (var fuel in tempData.SelectedFuels)
            {
                sb.AppendLine($"📍{fuel.FuelType.GetDescription()}  ");
            }

            sb.AppendLine();
            sb.AppendLine("\n<b>Оберіть пальне</b> (на клавіатурі), або натисніть <b>\"✅Підтвердити\"</b>");

            return sb.ToString();
        }

        private async Task<IReplyMarkup> GetCustomKeyboard()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);

            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "До головної" });

            if(tempData.SelectedFuels != null && tempData.SelectedFuels.Any())
            {
                keyboard.Add(new KeyboardButton[] { "✅Підтвердити", "⬅️Скинути обране пальне" });
            }

            if (tempData.SelectedGasStation != null)
            {
                if(tempData.SelectedFuels == null)
                {
                    tempData.SelectedFuels = new List<Fuel>();
                }

                var availableFuel = new List<Fuel>();
                foreach (var fuel in tempData.SelectedGasStation!.Fuels)
                {
                    if(!tempData.SelectedFuels.Any(f => f.FuelType == fuel.FuelType))
                    {
                        availableFuel.Add(fuel);
                    }
                }

                for (int i = 0; i < availableFuel.Count(); i += 2)
                {
                    var subList = availableFuel.GetRange(i, Math.Min(2, availableFuel.Count() - i));
                    var subKeyboard = new KeyboardButton[subList.Count];
                    for (int j = 0; j < subList.Count; j++)
                    {
                        subKeyboard[j] = new KeyboardButton(subList[j].FuelType.GetDescription());
                    }

                    keyboard.Add(subKeyboard);
                }
            }

            return new ReplyKeyboardMarkup(keyboard);
        }
    }
}
