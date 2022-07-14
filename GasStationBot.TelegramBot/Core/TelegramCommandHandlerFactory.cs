using GasStationBot.Application.Services.Telegram;
using GasStationBot.TelegramBot.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace GasStationBot.TelegramBot.Core
{
    public class TelegramCommandHandlerFactory : ITelegramCommandHandlerFactory
    {
        private readonly IServiceProvider _services;

        public TelegramCommandHandlerFactory(IServiceProvider services)
        {
            _services = services;
        }

        public ITelegramCommandHandler CreateHandler<T>(T telegramCommand) where T : ITelegramCommand
        {
            using(var scope = _services.CreateScope())
            {
                return telegramCommand switch
                {
                    StartMenuCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<StartMenuCommand>>()!,
                    NoneMenuCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<NoneMenuCommand>>()!,
                    AboutCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<AboutCommand>>()!,
                    ShowGasStationCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<ShowGasStationCommand>>()!,

                    AddGasStationSelectProviderCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<AddGasStationSelectProviderCommand>>()!,
                    AddGasStationSelectCityCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<AddGasStationSelectCityCommand>>()!,
                    AddGasStationSelectGasStationsCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<AddGasStationSelectGasStationsCommand>>()!,
                    AddGasStationSelectFuelCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<AddGasStationSelectFuelCommand>>()!,
                    AddGasStationConfirmGasStationCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<AddGasStationConfirmGasStationCommand>>()!,

                    RemoveGasStationListGasStationsCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<RemoveGasStationListGasStationsCommand>>()!,
                    RemoveGasStationRemoveGasStationsCommand => scope.ServiceProvider.GetService<ITelegramCommandHandler<RemoveGasStationRemoveGasStationsCommand>>()!,

                    _ => scope.ServiceProvider.GetService<ITelegramCommandHandler<NoneMenuCommand>>()!
                };
            }
        }
    }
}
