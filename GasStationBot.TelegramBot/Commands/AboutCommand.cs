using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class AboutCommand : BaseTelegramCommand
    {
        public AboutCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.About;

        public override UserState? NextState => UserState.None;

    }
}
