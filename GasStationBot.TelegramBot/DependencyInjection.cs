using GasStationBot.Application.Services.Telegram;
using GasStationBot.TelegramBot.CommandHandlers;
using GasStationBot.TelegramBot.Commands;
using GasStationBot.TelegramBot.Core;
using Microsoft.Extensions.DependencyInjection;

namespace GasStationBot.TelegramBot
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTelegramServices(this IServiceCollection services)
        {
            services.AddScoped<ITelegramUserStateService, TelegramUserStateService>();
            services.AddScoped<ITelegramCommandFactory, TelegramCommandFactory>();
            services.AddScoped<ITelegramMessageMediator, TelegramMessageMediator>();
            services.AddScoped<ITelegramHandlerService, TelegramHandlerService>();

            AddTelegramCommandHanlders(services);

            return services;
        }

        private static IServiceCollection AddTelegramCommandHanlders(IServiceCollection services)
        {
            services.AddScoped<ITelegramCommandHandler<StartMenuCommand>, StartMenuCommandHandler>();
            services.AddScoped<ITelegramCommandHandler<NoneMenuCommand>, NoneMenuCommandHandler>();
            services.AddScoped<ITelegramCommandHandler<AboutCommand>, AboutCommandHandler>();
            services.AddScoped<ITelegramCommandHandler<ShowGasStationCommand>, ShowGasStationCommandHandler>();
            
            services.AddScoped<ITelegramCommandHandler<AddGasStationSelectProviderCommand>, AddGasStationSelectProviderCommandHandler>();
            services.AddScoped<ITelegramCommandHandler<AddGasStationSelectCityCommand>, AddGasStationSelectCityCommandHandler>();
            services.AddScoped<ITelegramCommandHandler<AddGasStationSelectGasStationsCommand>, AddGasStationSelectGasStationsCommandHandler>();
            services.AddScoped<ITelegramCommandHandler<AddGasStationSelectFuelCommand>, AddGasStationSelectFuelCommandHandler>();
            services.AddScoped<ITelegramCommandHandler<AddGasStationConfirmGasStationCommand>, AddGasStationConfirmGasStationCommandHandler>();

            services.AddScoped<ITelegramCommandHandler<RemoveGasStationListGasStationsCommand>, RemoveGasStationListGasStationsCommandHandler>();
            services.AddScoped<ITelegramCommandHandler<RemoveGasStationRemoveGasStationsCommand>, RemoveGasStationRemoveGasStationsCommandHandler>();

            return services;
        }
    }
}
