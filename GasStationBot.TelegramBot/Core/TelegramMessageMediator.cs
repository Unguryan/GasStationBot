using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Core
{
    public class TelegramMessageMediator : ITelegramMessageMediator
    {

        private readonly ITelegramCommandHandlerFactory _handlerFactory;

        public TelegramMessageMediator(ITelegramCommandHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        public async Task<UserState> Send<T>(T telegramCommand)
            where T : ITelegramCommand
        {
            var handler = _handlerFactory.CreateHandler(telegramCommand);

            if (handler == null)
            {
                throw new ArgumentException("Handler does exist.");
            }

            var updatedState = await handler.Handle(telegramCommand);

            return updatedState;
        }

    }
}
