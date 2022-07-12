using System.ComponentModel;

namespace GasStationBot.Domain.Entities
{
    public enum FuelType
    {
        [Description("92")]
        Petrol_92,

        [Description("92+")]
        Petrol_92_Plus,

        [Description("95")]
        Petrol_95,

        [Description("95+")]
        Petrol_95_Plus,

        [Description("98")]
        Petrol_98,

        [Description("98+")]
        Petrol_98_Plus,

        [Description("100")]
        Petrol_100,

        [Description("100+")]
        Petrol_100_Plus,

        [Description("ДП")]
        Diesel,

        [Description("ДП+")]
        Diesel_Plus,

        [Description("Газ")]
        Gas
    }
}
