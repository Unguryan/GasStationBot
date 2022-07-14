using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class AddGasStationConfirmGasStationCommand : BaseTelegramCommand
    {
        public AddGasStationConfirmGasStationCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.AddGasStation_ConfirmGasStation;

        public override UserState? NextState => UserState.None;

    }
}
