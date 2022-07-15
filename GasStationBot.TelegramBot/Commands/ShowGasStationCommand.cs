using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class ShowGasStationCommand : BaseTelegramCommand
    {
        public ShowGasStationCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override bool IsMessageOnly => true;

        public override UserState UserState => UserState.ShowGasStation;

        public override UserState? NextState => UserState.None;

    }
}
