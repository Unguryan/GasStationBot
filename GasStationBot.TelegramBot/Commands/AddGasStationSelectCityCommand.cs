using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class AddGasStationSelectCityCommand : BaseTelegramCommand
    {
        public AddGasStationSelectCityCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.AddGasStation_SelectCity;

        public override UserState? NextState => UserState.AddGasStation_SelectGasStations;

    }
}
