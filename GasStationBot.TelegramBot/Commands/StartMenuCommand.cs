using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class StartMenuCommand : BaseTelegramCommand
    {
        public StartMenuCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.StartMenu;

        public override UserState? NextState => UserState.None;

    }
}
