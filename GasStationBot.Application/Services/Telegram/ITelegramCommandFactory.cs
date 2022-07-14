using GasStationBot.Domain.Entities;

namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramCommandFactory
    {
        ITelegramCommand? TryToCreateCommandByTelegramCommand(string userMessage, string userId);

        ITelegramCommand? TryToCreateCommandByUserState(UserState userState, string userId, string userMessage);

        ITelegramCommand? TryToCreateEmptyCommandByUserState(UserState userState, string userId);
    }
}
