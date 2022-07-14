using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;

namespace GasStationBot.TelegramBot.Core
{
    public class TelegramMessageMediator : ITelegramMessageMediator
    {

        private readonly IServiceContainer _serviceContainer;

        private readonly ITelegramCommandFactory _commandFactory;


        public TelegramMessageMediator(IServiceContainer service, ITelegramCommandFactory commandFactory)
        {
            _serviceContainer = service;
            _commandFactory = commandFactory;
        }

        public async Task<UserState> Send<T>(T telegramCommand)
            where T : ITelegramCommand
        {
            var handler = _serviceContainer.GetService<ITelegramCommandHandler<T>>();

            if(handler == null)
            {
                throw new ArgumentException("Handler does exist.");
            }

            var updatedState = await handler.Handle(telegramCommand);

            //TODO: Here create a new command, without any message, and handle it, just to see menu for new state 
            //var nextCommand = _commandFactory.TryToCreateEmptyCommandByUserState(updatedState, telegramCommand.UserId);
            //var type = nextCommand.GetType();
            //var nextHandler = _serviceContainer.GetService<ITelegramCommandHandler<type>>();

            //if (handler == null)
            //{
            //    throw new ArgumentException("Handler does exist.");
            //}

            //var updatedState = await handler.Handle(telegramCommand);

            return updatedState;
        }

    }
}
