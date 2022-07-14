using GasStationBot.Application.Services;
using GasStationBot.Domain.Entities;
using GasStationBot.Infrastructure.DB;
using Microsoft.EntityFrameworkCore;

namespace GasStationBot.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly GasStationContext _context;

        public UserService(GasStationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserById(string id)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> AddUser(User user)
        {
            var tempUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (tempUser != null)
            {
                return false;
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            //await _context.Users.AddAsync(user).AsTask()
            //    .ContinueWith(async (t) =>
            //    {
            //        await _context.SaveChangesAsync();
            //    });

            //try
            //{

            //    var res = _context.Users.Add(user);
            //    _context.SaveChanges();
            //}
            //catch(Exception e)
            //{
            //    return false;
            //}


            return true;
        }

        public async Task<bool> AddGasStationToUser(string userId, GasStation gasStation)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null ||
                user.GasStations == null ||
                user.GasStations.Any(g => g.City == gasStation.City
                                       && g.Address == gasStation.Address
                                       && g.Provider == gasStation.Provider))
            {
                return false;
            }

            user.GasStations.Add(gasStation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveGasStationFromUser(string userId, GasStation gasStation)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null ||
                user.GasStations == null ||
                !user.GasStations.Any(g => g.City == gasStation.City
                                        && g.Address == gasStation.Address
                                        && g.Provider == gasStation.Provider))
            {
                return false;
            }

            var stationToRemove = user.GasStations.First(g => g.City == gasStation.City
                                                           && g.Address == gasStation.Address
                                                           && g.Provider == gasStation.Provider);
            user.GasStations.Remove(stationToRemove);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateGasStationState(string userId, GasStation gasStation, List<Fuel> fuels)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null ||
                user.GasStations == null ||
                //TODO: Check how Equals will work for records
                !user.GasStations.Any(g => g.City == gasStation.City
                                        && g.Address == gasStation.Address
                                        && g.Provider == gasStation.Provider))
            {
                return false;
            }

            var stationToUpdate = user.GasStations.First(g => g.City == gasStation.City
                                                           && g.Address == gasStation.Address
                                                           && g.Provider == gasStation.Provider);

            stationToUpdate.Fuels.Clear();
            fuels.ForEach(f => stationToUpdate.Fuels.Add(f));
            await _context.SaveChangesAsync();
            return true;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
