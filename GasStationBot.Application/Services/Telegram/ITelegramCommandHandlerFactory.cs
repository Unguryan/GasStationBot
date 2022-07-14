namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramCommandHandlerFactory
    {
        ITelegramCommandHandler CreateHandler<T>(T telegramCommand) where T : ITelegramCommand;
        //ITelegramCommandHandler<T> CreateHandler<T>(T telegramCommand) where T : ITelegramCommand;
    }
}
