using GasStationBot.Domain.Entities;

namespace GasStationBot.Application.Services
{
    public interface IUserService
    {

        Task<IEnumerable<User>> GetAllUsers();

        Task<User> GetUserById(string id);

        Task<bool> AddUser(User user);

        Task<bool> AddGasStationToUser(string userId, GasStation station);

        Task<bool> RemoveGasStationFromUser(string userId, GasStation station);

        Task<bool> UpdateGasStationState(string userId, GasStation station, List<Fuel> fuels);

    }
}
