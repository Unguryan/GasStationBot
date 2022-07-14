using GasStationBot.Domain.Entities;

namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramMessageMediator
    {
        Task<UserState> Send<T>(T telegramCommand) where T : ITelegramCommand;
    }
}
