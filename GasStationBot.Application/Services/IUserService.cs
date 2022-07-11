using GasStationBot.Application.Models;

namespace GasStationBot.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<IUser>> GetAllUsers();

        Task<bool> AddUser(IUser user);

        Task<bool> AddGasStationToUser(IUser user, IGasStation station);

        Task<bool> RemoveGasStationFromUser(IUser user, IGasStation station);
    }
}
