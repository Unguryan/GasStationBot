using GasStationBot.Domain.Entities;

namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramCommand
    {
        string UserId { get; }

        UserState UserState { get; }

        //IReplyMarkup? Keyboard { get; }

        UserState? NextState { get; }

        string UserMessage { get; }

    }
}