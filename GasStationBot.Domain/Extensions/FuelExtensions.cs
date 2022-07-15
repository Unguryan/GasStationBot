using GasStationBot.Domain.Entities;
using System.ComponentModel;
using System.Reflection;

namespace GasStationBot.Domain.Extensions
{
    public static class FuelExtensions
    {
        //TODO: Refactor it
        //public static string GetDescription(this FuelType fuelType)
        //{
        //    //Enums.GetEnumDescription
        //    Type type = fuelType.GetType();
        //    var res = type.GetCustomAttributes(typeof(DescriptionAttribute), false)
        //                  .FirstOrDefault() as DescriptionAttribute;

        //    return res != null ? res.Description : string.Empty;
        //}

        //public static string GetDescription(this FuelState fuelState)
        //{
        //    Type type = fuelState.GetType();
        //    var res = type.GetCustomAttributes(typeof(DescriptionAttribute), false)
        //                  .FirstOrDefault() as DescriptionAttribute;

        //    return res != null ? res.Description : string.Empty;
        //}

        public static string GetDescription<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return source.ToString();
            }
        }
    }
}
