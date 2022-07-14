using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class NoneMenuCommandHandler : BaseTelegramCommandHandler<NoneMenuCommand>
    {
        private readonly ITelegramUserStateService _userStateService;

        public NoneMenuCommandHandler(ITelegramBotClient botClient,
                                      ITelegramUserStateService userStateService) : base(botClient)
        {
            _userStateService = userStateService;
        }

        protected override string Message => "Головне меню:";

        protected override IReplyMarkup Keyboard => new ReplyKeyboardMarkup(
                    new[]
                    {
                        new KeyboardButton[] { "Ваші підписки", },
                        new KeyboardButton[] { "Додати підписку", },
                        new KeyboardButton[] { "Видалити підписку", },
                        new KeyboardButton[] { "Про бота", },
                    })
        {
            ResizeKeyboard = true
        };

        protected override async Task<UserState> HandleCommand()
        {
            _userStateService.ClearTempData(Command.UserId);
            var state = Command.UserMessage switch
            {
                "Ваші підписки" => UserState.ShowGasStation,
                "Додати підписку" => UserState.AddGasStation_SelectProvider,
                "Видалити підписку" => UserState.RemoveGasStation_ListGasStations,
                "Про бота" => UserState.About,
                _ => Command.UserState
            };

            if(state == Command.UserState)
            {
                await SendMessage(Command.UserId, Message, Keyboard);
                return state;
            }

            return Command.NextState!.Value;
        }
    }
}
