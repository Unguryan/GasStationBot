using GasStationBot.Application.Services;
using GasStationBot.Infrastructure.DB;
using GasStationBot.Infrastructure.Services;
using GasStationBot.TelegramBot;
using GasStationBot.WOG_Station.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GasStationBot.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddWogGasStation();
            services.AddOkkoGasStation();

            services.AddDbContext<GasStationContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INotifyUserService, NotifyUserService>();

            services.AddScoped<IGasStationBackgroundWorker, GasStationBackgroundWorker>();
            services.AddHostedService<GasStationBackgroundWorkerHostedService>();

            services.AddTelegramServices();

            return services;
        }
    }
}
