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

        protected override string Message => GetCustomMessage();

        protected override IReplyMarkup Keyboard => GetCustomKeyboard();

        protected override async Task<UserState> HandleCommand()
        {
            if (CheckBackCommand())
            {
                return UserState.None;
            }

            var userTask = _userService.GetUserById(Command.UserId);
            userTask.Wait();
            var user = userTask.Result;
            if(user.GasStations == null || !user.GasStations.Any())
            {
                await SendMessage(Command.UserId, "Ви ще не додали жодної АЗС.");
                return UserState.None;
            }


            if (int.TryParse(Command.UserMessage, out int gasStationId) && 
                CheckID(gasStationId, out GasStation gasStation))
            {
                _userStateService.SetUserTempData(Command.UserId, new TempUserDataModel() { SelectedGasStation = gasStation });
                return Command.NextState!.Value;
            }

            await SendMessage(Command.UserId, "АЗС з таким ІД не знайдено, спробуйте ще.");
            await SendMessage(Command.UserId, Message, Keyboard);
            return Command.UserState;
        }

        private bool CheckID(int gasStationId, out GasStation gasStation)
        {
            var userTask = _userService.GetUserById(Command.UserId);
            userTask.Wait();
            var user = userTask.Result;

            if (gasStationId <= 0 || gasStationId > user.GasStations.Count)
            {
                gasStation = null;
                return false;
            }

            gasStation = user.GasStations[gasStationId - 1];
            return true;
        }

        private string GetCustomMessage()
        {
            //TODO: Fix it
            var userTask = _userService.GetUserById(Command.UserId);
            userTask.Wait();
            var user = userTask.Result;

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
                sb.AppendLine($"Провадер: {gs.Provider}");
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

        private IReplyMarkup GetCustomKeyboard()
        {
            var userTask = _userService.GetUserById(Command.UserId);
            userTask.Wait();
            var user = userTask.Result;
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
