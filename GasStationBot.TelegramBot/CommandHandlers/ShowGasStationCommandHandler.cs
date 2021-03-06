using GasStationBot.Application.Services;
using GasStationBot.Domain.Entities;
using GasStationBot.Domain.Extensions;
using GasStationBot.TelegramBot.Commands;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class ShowGasStationCommandHandler : BaseTelegramCommandHandlerWithContext<ShowGasStationCommand>
    {
        public ShowGasStationCommandHandler(ITelegramBotClient botClient) : base(botClient)
        {
        }

        protected override Task<string> Message => GetCustomMessage();

        protected override Task<IReplyMarkup> Keyboard => Task.FromResult<IReplyMarkup>(null);

        protected override async Task<UserState> HandleCommand()
        {
            await SendMessage(Command.UserId, await Message);
            return Command.NextState!.Value;
        }

        private async Task<string> GetCustomMessage()
        {
            var user = await UserService.GetUserById(Command.UserId);

            if (user.GasStations == null ||
               !user.GasStations.Any())
            {
                return "❌У вас ще нема підписок❌";
            }

            var sb = new StringBuilder();
            sb.AppendLine("<b>Ваші підписки: </b>");
            sb.AppendLine();
            var counter = 1;
            foreach (var gs in user.GasStations)
            {
                sb.AppendLine($"✅ІД: {counter++}");
                sb.AppendLine($"Власник АЗС: <b>{gs.Provider}</b>");
                sb.AppendLine($"Місто: <b>{gs.City}</b>");
                sb.AppendLine($"Адреса: <b>{gs.Address}</b>");

                var sbFuel = new StringBuilder();
                sbFuel.AppendLine($"\n<b>Пальне</b> (яке відстежується): ");
                for (int i = 0; i < gs.Fuels.Count; i++)
                {
                    var sbState = new StringBuilder();
                    sbFuel.Append("<b>📍" + gs.Fuels[i].FuelType.GetDescription() + "</b> - ");
                    for (int j = 0; j < gs.Fuels[i].StateOfFuel.Count; j++)
                    {
                        sbState.Append(gs.Fuels[i].StateOfFuel[j].GetDescription());
                        if (j != gs.Fuels[i].StateOfFuel.Count - 1)
                        {
                            sbState.Append(", ");
                        }
                    }
                    sbFuel.Append(sbState.ToString() + "\n");
                }

                sb.AppendLine(sbFuel.ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
