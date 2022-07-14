using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class AddGasStationSelectGasStationsCommand : BaseTelegramCommand
    {
        public AddGasStationSelectGasStationsCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.AddGasStation_SelectGasStations;

        public override UserState? NextState => UserState.AddGasStation_SelectFuel;

    }
}
