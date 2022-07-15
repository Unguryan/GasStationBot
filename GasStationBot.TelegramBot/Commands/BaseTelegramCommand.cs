using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public abstract class BaseTelegramCommand : ITelegramCommand
    {

        protected BaseTelegramCommand(string userId, string userMessage)
        {
            UserId = userId;
            UserMessage = userMessage;
        }

        public string UserId { get; }

        public string UserMessage { get; }

        public virtual bool IsMessageOnly => false;

        public abstract UserState UserState { get; }

        public abstract UserState? NextState { get; }

    }
}
