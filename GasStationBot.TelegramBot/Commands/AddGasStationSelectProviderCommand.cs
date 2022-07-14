using GasStationBot.Domain.Entities;

namespace GasStationBot.TelegramBot.Commands
{
    public class AddGasStationSelectProviderCommand : BaseTelegramCommand
    {
        public AddGasStationSelectProviderCommand(string userId, string userMessage) : base(userId, userMessage)
        {
        }

        public override UserState UserState => UserState.AddGasStation_SelectProvider;

        public override UserState? NextState => UserState.AddGasStation_SelectCity;

    }
}
