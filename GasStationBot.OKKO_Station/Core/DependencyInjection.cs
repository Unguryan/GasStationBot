using GasStationBot.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GasStationBot.WOG_Station.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOkkoGasStation(this IServiceCollection services)
        {
            services.AddScoped<IGasStationsService, OKKO_GasStationService>();
            return services;
        }
    }
}
