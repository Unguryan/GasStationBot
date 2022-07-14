using GasStationBot.Domain.Entities;

namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramCommandHandler<T> where T : ITelegramCommand
    {
        Task<UserState> Handle(T command);
    }
}
