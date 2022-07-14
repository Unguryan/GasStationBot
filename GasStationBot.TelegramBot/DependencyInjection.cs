using GasStationBot.Application.Services.Telegram;
using GasStationBot.TelegramBot.Core;
using Microsoft.Extensions.DependencyInjection;

namespace GasStationBot.TelegramBot
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTelegramServices(this IServiceCollection services)
        {
            services.AddScoped<ITelegramHandlerService, TelegramHandlerService>();

            AddTelegramCommandHanlders(services);

            return services;
        }

        private static IServiceCollection AddTelegramCommandHanlders(IServiceCollection services)
        {
            //services.AddScoped<>();


            return services;
        }
    }
}
