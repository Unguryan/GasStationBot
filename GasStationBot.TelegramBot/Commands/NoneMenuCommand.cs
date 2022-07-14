using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class NoneMenuCommand : BaseTelegramCommand
    {
        public NoneMenuCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.None;

        public override UserState? NextState => UserState.None;

    }
}
