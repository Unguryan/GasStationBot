using GasStationBot.Domain.Entities;
using System.ComponentModel;

namespace GasStationBot.Domain.Extensions
{
    public static class FuelExtensions
    {
        //TODO: Refactor it
        public static string GetDescription(this FuelType fuelType)
        {
            Type type = fuelType.GetType();
            var res = type.GetCustomAttributes(typeof(DescriptionAttribute), false)
                          .FirstOrDefault() as DescriptionAttribute;

            return res != null ? res.Description : string.Empty;
        }

        public static string GetDescription(this FuelState fuelState)
        {
            Type type = fuelState.GetType();
            var res = type.GetCustomAttributes(typeof(DescriptionAttribute), false)
                          .FirstOrDefault() as DescriptionAttribute;

            return res != null ? res.Description : string.Empty;
        }
    }
}
