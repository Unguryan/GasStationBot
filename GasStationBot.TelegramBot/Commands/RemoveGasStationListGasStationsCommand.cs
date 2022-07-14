using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class RemoveGasStationListGasStationsCommand : BaseTelegramCommand
    {
        public RemoveGasStationListGasStationsCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.RemoveGasStation_ListGasStations;

        public override UserState? NextState => UserState.RemoveGasStation_RemoveGasStations;

    }
}
