using GasStationBot.Application.Services;
using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Extensions;
using GasStationBot.TelegramBot.Commands;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class ShowGasStationCommandHandler : BaseTelegramCommandHandler<ShowGasStationCommand>
    {
        private readonly IUserService _userService;

        public ShowGasStationCommandHandler(ITelegramBotClient botClient, IUserService userService) : base(botClient)
        {
            _userService = userService;
        }

        protected override Task<string> Message => GetCustomMessage();

        protected override Task<IReplyMarkup> Keyboard => null;

        protected override async Task<UserState> HandleCommand()
        {
            await SendMessage(Command.UserId, await Message);
            return Command.NextState!.Value;
        }

        private async Task<string> GetCustomMessage()
        {
            var user = await _userService.GetUserById(Command.UserId);

            if (user.GasStations == null ||
               !user.GasStations.Any())
            {
                return "У вас ще нема підписок";
            }

            var sb = new StringBuilder();
            sb.AppendLine("Ваші підписки: ");
            sb.AppendLine();
            var counter = 1;
            foreach (var gs in user.GasStations)
            {
                sb.AppendLine($"ІД: {counter++}");
                sb.AppendLine($"Власник АЗС: {gs.Provider}");
                sb.AppendLine($"Місто: {gs.City}");
                sb.AppendLine($"Адреса: {gs.Address}");

                var sbFuel = new StringBuilder();
                sbFuel.Append($"Паливо (яке відстежується): ");
                for (int i = 0; i < gs.Fuels.Count; i++)
                {
                    sbFuel.Append(gs.Fuels[i].FuelType.GetDescription());
                    if (i != gs.Fuels.Count - 1)
                    {
                        sbFuel.Append(", ");
                    }
                }

                sb.AppendLine(sbFuel.ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
