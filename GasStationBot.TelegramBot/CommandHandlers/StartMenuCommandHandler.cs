using GasStationBot.Domain.Entities;
using GasStationBot.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class StartMenuCommandHandler : BaseTelegramCommandHandler<StartMenuCommand>
    {
        public StartMenuCommandHandler(ITelegramBotClient botClient) : base(botClient)
        {
        }

        protected override string Message => "Добрий День!\nЦе телеграм бот, який потрібен для відстеження явності пального в Україні.";

        protected override IReplyMarkup Keyboard => null;

        protected override async Task<UserState> HandleCommand()
        {
            await SendMessage(Command.UserId, Message);

            return Command.NextState!.Value;
        }
    }
}
