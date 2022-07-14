using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class AddGasStationSelectFuelCommand : BaseTelegramCommand
    {
        public AddGasStationSelectFuelCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.AddGasStation_SelectFuel;

        public override UserState? NextState => UserState.AddGasStation_ConfirmGasStation;

    }
}
