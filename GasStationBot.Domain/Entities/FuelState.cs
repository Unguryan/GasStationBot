using System.ComponentModel;

namespace GasStationBot.Domain.Entities
{
    public enum FuelState
    {
        [Description("за готівку/карту")]
        Exist,

        [Description("за талони")]
        Talons,

        [Description("для спец. техніки")]
        OnlyForSpecVehicles,

        [Description("Паливо відсутнє")]
        NotExist,
    }
}
