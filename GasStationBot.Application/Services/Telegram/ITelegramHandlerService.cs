using Telegram.Bot.Types;

namespace GasStationBot.Application.Services.Telegram
{
    public interface ITelegramHandlerService
    {
        Task HandleMessageAsync(Update update);

        //Task<bool> SendMessage(string userId, string message, IReplyMarkup? keyboard = null);

    }
}
