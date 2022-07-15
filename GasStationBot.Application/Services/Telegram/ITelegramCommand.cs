using GasStationBot.Domain.Entities;

namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramCommand
    {
        string UserId { get; }

        string UserMessage { get; }

        bool IsMessageOnly { get; }

        UserState UserState { get; }

        //IReplyMarkup? Keyboard { get; }

        UserState? NextState { get; }


    }
}