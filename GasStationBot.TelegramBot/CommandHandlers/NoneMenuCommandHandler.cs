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

        protected override Task<string> Message => Task.FromResult("Головне меню:");

        protected override Task<IReplyMarkup> Keyboard => GetCustomKeyboard();

        private async Task<IReplyMarkup> GetCustomKeyboard()
        {
            return new ReplyKeyboardMarkup(
               new[]
               {
                   new KeyboardButton[] { "Мої підписки", },
                   new KeyboardButton[] { "Додати підписку", },
                   new KeyboardButton[] { "Видалити підписку", },
                   new KeyboardButton[] { "Про бота", },
               })
            {
                ResizeKeyboard = true
            };
        }

        protected override async Task<UserState> HandleCommand()
        {
            _userStateService.ClearTempData(Command.UserId);
            var state = Command.UserMessage switch
            {
                "Мої підписки" => UserState.ShowGasStation,
                "Додати підписку" => UserState.AddGasStation_SelectProvider,
                "Видалити підписку" => UserState.RemoveGasStation_ListGasStations,
                "Про бота" => UserState.About,
                _ => Command.UserState
            };

            if (state == Command.UserState)
            {
                await SendMessage(Command.UserId, await Message, await Keyboard);
                return state;
            }

            return state;
        }
    }
}
