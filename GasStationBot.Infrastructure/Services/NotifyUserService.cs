using GasStationBot.Application.Services;
using GasStationBot.Domain.Entities;

namespace GasStationBot.Infrastructure.Services
{
    public class NotifyUserService : INotifyUserService
    {

        private readonly IUserService _userService;

        private readonly IEnumerable<IGasStationsService> gasStationsServices;

        public NotifyUserService(IUserService userService,
                                 IEnumerable<IGasStationsService> gasStationsServices)
        {
            _userService = userService;
            this.gasStationsServices = gasStationsServices;
        }

        public async Task NotifyAllUsers()
        {
            var users = await _userService.GetAllUsers();
            var gasStations = new List<GasStation>();

            foreach (var gasStationsService in gasStationsServices)
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
                    await _userService.UpdateGasStationState(selectedUser.Id, userGasStation, updatedFuels);
                }
            }
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
    }
}
