using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.TelegramBot.Commands;

namespace GasStationBot.TelegramBot.Core
{
    public class TelegramCommandFactory : ITelegramCommandFactory
    {
        public ITelegramCommand? TryToCreateCommandByTelegramCommand(string userMessage, string userId)
        {
            return userMessage switch
            {
                "/start" => new StartMenuCommand(userId, userMessage),
                "/menu" => new NoneMenuCommand(userId, userMessage),
                "/about" => new AboutCommand(userId, userMessage),
                "/add" => new AddGasStationSelectProviderCommand(userId, userMessage),
                "/remove" => new RemoveGasStationListGasStationsCommand(userId, userMessage),
                "/show" => new ShowGasStationCommand(userId, userMessage),
                _ => null
            };
        }

        public ITelegramCommand? TryToCreateCommandByUserState(UserState userState, string userId, string userMessage)
        {
            return userState switch
            {
                UserState.StartMenu => new StartMenuCommand(userId, userMessage),
                UserState.None => new NoneMenuCommand(userId, userMessage),
                UserState.About => new AboutCommand(userId, userMessage),
                UserState.ShowGasStation => new ShowGasStationCommand(userId, userMessage),

                UserState.AddGasStation_SelectProvider => new AddGasStationSelectProviderCommand(userId, userMessage),
                UserState.AddGasStation_SelectCity => new AddGasStationSelectCityCommand(userId, userMessage),
                UserState.AddGasStation_SelectGasStations => new AddGasStationSelectGasStationsCommand(userId, userMessage),
                UserState.AddGasStation_SelectFuel => new AddGasStationSelectFuelCommand(userId, userMessage),
                UserState.AddGasStation_ConfirmGasStation => new AddGasStationConfirmGasStationCommand(userId, userMessage),

                UserState.RemoveGasStation_ListGasStations => new RemoveGasStationListGasStationsCommand(userId, userMessage),
                UserState.RemoveGasStation_RemoveGasStations => new RemoveGasStationRemoveGasStationsCommand(userId, userMessage),

                _ => null
            };
        }

        public ITelegramCommand? TryToCreateEmptyCommandByUserState(UserState userState, string userId)
        {
            return TryToCreateCommandByUserState(userState, userId, null);
        }
    }
}
