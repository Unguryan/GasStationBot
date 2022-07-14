using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Entities.Temp;

namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramUserStateService
    {

        UserState? GetUserState(string userId);

        void SetUserState(string userId, UserState state);

        TempUserDataModel GetUserTempData(string userId);

        void SetUserTempData(string userId, TempUserDataModel tempData);

        void ClearTempData(string userId);
    }
}
