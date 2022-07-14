using GasStationBot.Application.Services;
using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Extensions;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GasStationBot.Infrastructure.Services
{
    public class NotifyUserService : INotifyUserService
    {

        private readonly IUserService _userService;

        private readonly IEnumerable<IGasStationsService> _gasStationsServices;

        private readonly ITelegramBotClient _botClient;

        public NotifyUserService(IUserService userService,
                                 IEnumerable<IGasStationsService> gasStationsServices,
                                 ITelegramBotClient botClient)
        {
            _userService = userService;
            _gasStationsServices = gasStationsServices;
            _botClient = botClient;
        }

        public async Task NotifyAllUsers()
        {
            var users = await _userService.GetAllUsers();
            var gasStations = new List<GasStation>();

            foreach (var gasStationsService in _gasStationsServices)
            {
                gasStations.AddRange(await gasStationsService.GetGasStations());
            }

            //Gas station will be less, for this reason, we have to foreach gasStations

            foreach (var gasStation in gasStations)
            {
                var selectedUsers = users.Where(u => u.GasStations.Any(g => g.City == gasStation.City
                                                                         && g.Address == gasStation.Address
                                                                         && g.Provider == gasStation.Provider));

                if (!selectedUsers.Any())
                {
                    continue;
                }

                foreach (var selectedUser in selectedUsers)
                {
                    var userGasStation = selectedUser.GasStations.First(g => g.City == gasStation.City
                                                                          && g.Address == gasStation.Address
                                                                          && g.Provider == gasStation.Provider);

                    var updatedFuels = GetUpdatedFuels(gasStation, userGasStation);
                    if (updatedFuels == null || !updatedFuels.Any())
                    {
                        continue;
                    }

                    //TODO: Use Telegram API, to send updated info for users
                    var result = await _userService.UpdateGasStationState(selectedUser.Id, userGasStation, updatedFuels);
                    if (result)
                    {
                        await NotifyUser(selectedUser, userGasStation, updatedFuels);
                    }
                }
            }
        }

        private async Task NotifyUser(Domain.Entities.User selectedUser,
                                      GasStation userGasStation,
                                      List<Fuel> updatedFuels)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Апдейт по АЗС!");

            sb.AppendLine($"Власник АЗС: {userGasStation.Provider}");
            sb.AppendLine($"Місто: {userGasStation.City}");
            sb.AppendLine($"Адреса: {userGasStation.Address}");
            sb.AppendLine();

            sb.AppendLine($"Оновлене Паливо: ");

            foreach (var fuel in updatedFuels)
            {
                var sbFuelState = new StringBuilder();

                sbFuelState.Append($"Паливо {fuel.FuelType} - ");
                for (int i = 0; i < fuel.StateOfFuel.Count; i++)
                {
                    sbFuelState.Append($"{fuel.StateOfFuel[i].GetDescription()}");
                    if (i != fuel.StateOfFuel.Count - 1)
                    {
                        sbFuelState.Append($", ");
                    }
                }
                
                sb.AppendLine(sbFuelState.ToString());
            }

            await SendMessage(selectedUser.Id, sb.ToString());
        }

        private List<Fuel> GetUpdatedFuels(GasStation gasStation, GasStation userGasStation)
        {
            var result = new List<Fuel>();
            foreach (var userFuel in userGasStation.Fuels)
            {
                var tempGasStationFuel = gasStation.Fuels.FirstOrDefault(f => f.FuelType == userFuel.FuelType);
                if (tempGasStationFuel == null || !CompareFuels(tempGasStationFuel, userFuel))
                {
                    continue;
                }

                result.Add(tempGasStationFuel);
            }

            return result;
        }

        //return true - if same,
        //return false - if have updates
        private bool CompareFuels(Fuel gasStationFuel, Fuel userFuel)
        {
            if (gasStationFuel.StateOfFuel.Count != userFuel.StateOfFuel.Count)
            {
                return false;
            }

            var result = true;
            foreach (var state in userFuel.StateOfFuel)
            {
                result = gasStationFuel.StateOfFuel.Any(st => st == state);
                if (!result)
                {
                    break;
                }
            }

            return result;
        }

        private async Task<bool> SendMessage(string userId, string message)
        {
            try
            {
                await _botClient.SendTextMessageAsync(new ChatId(long.Parse(userId)), message);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
