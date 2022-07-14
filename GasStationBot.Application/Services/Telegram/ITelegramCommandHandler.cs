using GasStationBot.Domain.Entities;

namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramCommandHandler<T> : ITelegramCommandHandler where T : ITelegramCommand
    {
        T Command { get; }
    }

    public interface ITelegramCommandHandler
    {
        Task<UserState> Handle(ITelegramCommand command);
    }
}
