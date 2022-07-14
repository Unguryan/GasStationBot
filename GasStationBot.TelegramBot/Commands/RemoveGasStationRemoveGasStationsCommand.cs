using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class RemoveGasStationRemoveGasStationsCommand : BaseTelegramCommand
    {
        public RemoveGasStationRemoveGasStationsCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.RemoveGasStation_RemoveGasStations;

        public override UserState? NextState => UserState.None;

    }
}
