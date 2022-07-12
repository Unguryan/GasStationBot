using GasStationBot.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GasStationBot.WOG_Station.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWogGasStation(this IServiceCollection services)
        {
            services.AddScoped<IGasStationsService, WOG_GasStationService>();
            return services;
        }
    }
}
