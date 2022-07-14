using GasStationBot.Domain.Entities;
using GasStationBot.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GasStationBot.TelegramBot.CommandHandlers
{
    public class AboutCommandHandler : BaseTelegramCommandHandler<AboutCommand>
    {
        public AboutCommandHandler(ITelegramBotClient botClient) : base(botClient)
        {
        }

        protected override Task<string> Message => Task.FromResult("Цей телеграм бот, який сам слідкує за паливом на АЗС, та сповіщає вас, коли необхідне вам паливо з'явилось, або закінчилось.\n" +
            "У боті ви можете підписатись на різні АЗС в Україні, і після цього сам бот буде вас сповіщати, коли будуть оновлення на ваших АЗС." +
            "Це тільки бета-версія бота, Але якщо ви хочете подякувати автору або запропонувати нову ідею для бота, то ось його телеграм: @unguryan, та карта для донатів:" +
            "4149 4991 5680 6653 - Приват.\n" +
            "Віримо у Перемогу! Віримо в ЗСУ!\nСлава Україні!");

        protected override Task<IReplyMarkup> Keyboard => null;

        protected override async Task<UserState> HandleCommand()
        {
            await SendMessage(Command.UserId, await Message);

            return Command.NextState!.Value;
        }
    }
}
