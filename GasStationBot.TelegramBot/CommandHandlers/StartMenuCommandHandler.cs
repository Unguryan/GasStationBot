using GasStationBot.Domain.Entities;
using GasStationBot.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class StartMenuCommandHandler : BaseTelegramCommandHandler<StartMenuCommand>
    {
        public StartMenuCommandHandler(ITelegramBotClient botClient) : base(botClient)
        {
        }

        protected override Task<string> Message => Task.FromResult(
            "<b>Привіт !</b>\n"
            + "Шукаєш пальне ?⛽️\n\n"
            + "Я тобі допоможу <b>швидко орієнтуватись серед АЗС</b> й моніторити новини <b>не виходячи з дому</b> 😉\n\n"
            + "Просто <b>слідкуй за підказками</b>, обирай потрібний вид палива й заправку, а я буду інформувати тебе про ситуацію з ним 🔈\n\n"
            + "<b>Де ?</b>\n"
            + "У будь - якому місті України 🇺🇦\n\n"
            + "<b>Яке пальне ? 🚘</b>\n"
            + " • 95 +\n"
            + " • 95  \n"
            + " • ДП +\n"
            + " • ДП  \n"
            + " • Газ \n"
            + "\nЦе тільки <b>бета - версія бота</b>. Ми постійно працюємо, щоб його використання була комфортним для Вас!\n\n"
            + "Відкриті до співпраці, відгуків та пропозицій <b>Telegram</b> @unguryan 📩\n\n"
            + "<b>Підтримати автора</b> донатом можна тут 4149 4991 5680 6653 💸\n\n"
            + "<b>Віримо у Перемогу! Віримо в ЗСУ!</b>\n"
            + "<b>Слава Україні! 🇺🇦</b>");

        protected override Task<IReplyMarkup> Keyboard => Task.FromResult<IReplyMarkup>(null);

        protected override async Task<UserState> HandleCommand()
        {
            await SendMessage(Command.UserId, await Message);

            return Command.NextState!.Value;
        }
    }
}
