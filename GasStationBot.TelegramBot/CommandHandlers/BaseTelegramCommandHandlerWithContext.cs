using GasStationBot.Application.Services;
using GasStationBot.Application.Services.Telegram;
using Telegram.Bot;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public abstract class BaseTelegramCommandHandlerWithContext<T> : BaseTelegramCommandHandler<T>, ITelegramCommandHandlerWithContext<T> where T : ITelegramCommand
    {
        protected BaseTelegramCommandHandlerWithContext(ITelegramBotClient botClient) : base(botClient)
        {
        }

        protected IUserService UserService { get; private set; }

        public void InjectUserService(IUserService userService)
        {
            UserService = userService;
        }
    }
}
