namespace GasStationBot.Domain.Entities.Temp
{
    public class UserStateDataModel
    {
        public string Id { get; set; }

        public UserState UserState { get; set; }

        public TempUserDataModel TempData { get; set; }
    }
}
