namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramCommandHandlerWithContext<T> : ITelegramCommandHandlerWithContext, ITelegramCommandHandler<T> where T : ITelegramCommand
    {
    }

    public interface ITelegramCommandHandlerWithContext
    {
        void InjectUserService(IUserService userService);
    }
}
