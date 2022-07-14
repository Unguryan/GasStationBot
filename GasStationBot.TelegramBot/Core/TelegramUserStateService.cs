using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Entities.Temp;
using Newtonsoft.Json;

namespace GasStationBot.TelegramBot.Core
{
    public class TelegramUserStateService : ITelegramUserStateService
    {
        private const string Path = "UsersData.txt";

        private object locker = new object();

        public UserState? GetUserState(string userId)
        {
            lock (locker)
            {
                if (!File.Exists(Path))
                {
                    File.Create(Path).Dispose();
                };

                var json = File.ReadAllText(Path);
                var users = JsonConvert.DeserializeObject<IEnumerable<UserStateDataModel>>(json)?.ToList() ?? new List<UserStateDataModel>();
                return users?.SingleOrDefault(u => u.Id == userId)?.UserState ?? null;
            }
        }

        //return true - if user exist, false - if does not exist
        public void SetUserState(string userId, UserState state)
        {
            lock (locker)
            {
                if (!File.Exists(Path))
                {
                    File.Create(Path).Dispose();
                };

                var json = File.ReadAllText(Path);
                var users = JsonConvert.DeserializeObject<IEnumerable<UserStateDataModel>>(json)?.ToList() ?? new List<UserStateDataModel>();
                var user = users.SingleOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    users.Add(new UserStateDataModel() { Id = userId, UserState = state, TempData = new TempUserDataModel() });
                    json = JsonConvert.SerializeObject(users);
                    File.WriteAllText(Path, json);
                    return;
                }

                var userIndex = users.IndexOf(user);
                users[userIndex].UserState = state;

                json = JsonConvert.SerializeObject(users);
                File.WriteAllText(Path, json);
                return;
            }
        }

        public TempUserDataModel GetUserTempData(string userId)
        {
            lock (locker)
            {
                if (!File.Exists(Path))
                {
                    File.Create(Path).Dispose();
                };

                var json = File.ReadAllText(Path);
                var users = JsonConvert.DeserializeObject<IEnumerable<UserStateDataModel>>(json)?.ToList() ?? new List<UserStateDataModel>();
                return users.SingleOrDefault(u => u.Id == userId)?.TempData ?? null;
            }
        }

        public void SetUserTempData(string userId, TempUserDataModel tempData)
        {
            lock (locker)
            {
                if (!File.Exists(Path))
                {
                    File.Create(Path).Dispose();
                };

                var json = File.ReadAllText(Path);
                var users = JsonConvert.DeserializeObject<IEnumerable<UserStateDataModel>>(json)?.ToList() ?? new List<UserStateDataModel>();
                var user = users.SingleOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    users.Add(new UserStateDataModel() { Id = userId, UserState = UserState.StartMenu, TempData = tempData });
                    json = JsonConvert.SerializeObject(users);
                    File.WriteAllText(Path, json);
                    return;
                }

                var userIndex = users.IndexOf(user);
                users[userIndex].TempData = tempData;

                json = JsonConvert.SerializeObject(users);
                File.WriteAllText(Path, json);
                return;
            }
        }

        public void ClearTempData(string userId)
        {
            lock (locker)
            {
                if (!File.Exists(Path))
                {
                    File.Create(Path).Dispose();
                };

                var json = File.ReadAllText(Path);
                var users = JsonConvert.DeserializeObject<IEnumerable<UserStateDataModel>>(json)?.ToList() ?? new List<UserStateDataModel>();
                var user = users.SingleOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    users.Add(new UserStateDataModel() { Id = userId, UserState = UserState.StartMenu, TempData = new TempUserDataModel() });
                    json = JsonConvert.SerializeObject(users);
                    File.WriteAllText(Path, json);
                    return;
                }

                var userIndex = users.IndexOf(user);
                users[userIndex].TempData = new TempUserDataModel();

                json = JsonConvert.SerializeObject(users);
                File.WriteAllText(Path, json);
                return;
            }
        }
    }
}
