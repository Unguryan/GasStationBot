using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public abstract class BaseTelegramCommandHandler<T> : ITelegramCommandHandler<T> where T : ITelegramCommand
    {
        private readonly ITelegramBotClient _botClient;

        protected BaseTelegramCommandHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        protected abstract Task<string> Message { get; }

        protected abstract Task<IReplyMarkup> Keyboard { get; }

        protected abstract Task<UserState> HandleCommand();

        public T Command { get; private set; }

        public async Task<UserState> Handle(ITelegramCommand command)
        {
            Command = (T)command;
            //TODO: Refactor this code 
            //if message == "", just show menu

            if (string.IsNullOrEmpty(Command.UserMessage))
            {
                await SendMessage(Command.UserId, await Message, await Keyboard);
                //return command.NextState!.Value;
                return Command.UserState;
            }

            var res = await HandleCommand();

            return res;
        }

        protected async Task<bool> SendMessage(string userId, string message, IReplyMarkup? keyboard = null)
        {
            try
            {
                await _botClient.SendTextMessageAsync(new ChatId(long.Parse(userId)), message, replyMarkup: keyboard);
            }
            catch(ApiRequestException e)
            {
                return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected bool CheckBackCommand()
        {
            return Command.UserMessage == "До головної";
        }
    }
}
