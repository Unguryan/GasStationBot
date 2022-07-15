using GasStationBot.Application.Services;
using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Core
{
    public class TelegramMessageMediator : ITelegramMessageMediator
    {

        private readonly ITelegramCommandHandlerFactory _handlerFactory;

        private IUserService _userService;

        public TelegramMessageMediator(ITelegramCommandHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        public void InjectUserService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserState> Send<T>(T telegramCommand)
            where T : ITelegramCommand
        {
            var handler = _handlerFactory.CreateHandler(telegramCommand, _userService);

            if (handler == null)
            {
                throw new ArgumentException("Handler does exist.");
            }

            var updatedState = await handler.Handle(telegramCommand);

            return updatedState;
        }
    }
}
