using GasStationBot.Application.Services;
using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class AddGasStationSelectCityCommandHandler : BaseTelegramCommandHandler<AddGasStationSelectCityCommand>
    {

        private readonly IEnumerable<IGasStationsService> _gasStationsServices;

        private readonly ITelegramUserStateService _userStateService;

        public AddGasStationSelectCityCommandHandler(ITelegramBotClient botClient,
                                                     IEnumerable<IGasStationsService> gasStationsServices,
                                                     ITelegramUserStateService userStateService) : base(botClient)
        {
            _gasStationsServices = gasStationsServices;
            _userStateService = userStateService;
        }

        protected override string Message => "Введіть місто (або оберіть на клавіатурі), де потрібно шукати АЗС: ";

        protected override IReplyMarkup Keyboard => GetCustomKeyboard();

        protected override async Task<UserState> HandleCommand()
        {
            if (CheckBackCommand())
            {
                return UserState.None;
            }

            if (CheckGasStationByCity())
            {
                var tempData = _userStateService.GetUserTempData(Command.UserId);
                tempData.City = Command.UserMessage;
                _userStateService.SetUserTempData(Command.UserId, tempData);
                return Command.NextState!.Value;
            }

            await SendMessage(Command.UserId, "Місто не знайдено, спробуйте ще.");

            await SendMessage(Command.UserId, Message, Keyboard);
            return Command.UserState;
        }

        private bool CheckGasStationByCity()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);
            var gsService = _gasStationsServices.SingleOrDefault(u => u.GasStationName == tempData.ProviderName);
            if (gsService == null)
            {
                return false;
            }

            var citiesTask = gsService.GetGasStationsWithoutAdditionalData();
            citiesTask.Wait();
            var cities = citiesTask.Result.Select(gs => gs.City).Distinct().ToList();

            return cities.Any(c => c == Command.UserMessage);
        }

        private IReplyMarkup GetCustomKeyboard()
        {
            var tempData = _userStateService.GetUserTempData(Command.UserId);

            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "До головної" });
            var gsService = _gasStationsServices.SingleOrDefault(u => u.GasStationName == tempData.ProviderName);
            if (gsService != null)
            {
                //TODO: Fix Issue with .Result
                var citiesTask = gsService.GetGasStationsWithoutAdditionalData();
                citiesTask.Wait();
                var cities = citiesTask.Result.Select(gs => gs.City).Distinct().ToList();

                for (int i = 0; i < cities.Count(); i += 3)
                {
                    var subKeyboard = new KeyboardButton[3];
                    var subList = cities.GetRange(i, Math.Min(3, cities.Count() - i));
                    for (int j = 0; j < subList.Count; j++)
                    {
                        subKeyboard[j] = new KeyboardButton(subList[j]);
                    }

                    keyboard.Add(subKeyboard);
                }
            }

            return new ReplyKeyboardMarkup(keyboard);
        }
    }
}
