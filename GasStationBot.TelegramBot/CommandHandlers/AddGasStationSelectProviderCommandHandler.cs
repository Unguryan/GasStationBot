using GasStationBot.Application.Services;
using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Entities.Temp;
using GasStationBot.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class AddGasStationSelectProviderCommandHandler : BaseTelegramCommandHandler<AddGasStationSelectProviderCommand>
    {

        private readonly IEnumerable<IGasStationsService> _gasStationsServices;

        private readonly ITelegramUserStateService _userStateService;

        public AddGasStationSelectProviderCommandHandler(ITelegramBotClient botClient,
                                                         IEnumerable<IGasStationsService> gasStationsServices,
                                                         ITelegramUserStateService userStateService) : base(botClient)
        {
            _gasStationsServices = gasStationsServices;
            _userStateService = userStateService;
        }

        protected override string Message => "Оберіть компанію АЗС(на клавіатурі): ";

        protected override IReplyMarkup Keyboard => GetCustomKeyboard();

        protected override async Task<UserState> HandleCommand()
        {
            if (CheckBackCommand())
            {
                return UserState.None;
            }
            //var state = Command.UserMessage switch
            //{
            //    "До головної" => UserState.None,
            //    _ => Command.UserState
            //};

            if (CheckProviders())
            {
                _userStateService.SetUserTempData(Command.UserId, new TempUserDataModel() { ProviderName = Command.UserMessage });
                return Command.NextState!.Value;
            }

            await SendMessage(Command.UserId, "АЗС не знайдена, спробуйте ще.");

            await SendMessage(Command.UserId, Message, Keyboard);
            return Command.UserState;
        }

        private bool CheckProviders()
        {
            return _gasStationsServices.Any(x => x.GasStationName == Command.UserMessage);
        }

        private IReplyMarkup GetCustomKeyboard()
        {
            var keyboard = new List<KeyboardButton[]>();
            keyboard.Add(new KeyboardButton[] { "До головної" });
            foreach (var gsService in _gasStationsServices)
            {
                keyboard.Add(new KeyboardButton[] { gsService.GasStationName });
            }

            return new ReplyKeyboardMarkup(keyboard);
        }
    }
}
