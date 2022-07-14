using GasStationBot.Application.Services.Telegram;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace GasStationBot.WebHook.Controllers
{
    //[ApiController]
    //[Route("api/bot")]
    public class WebhookController : ControllerBase
    {
        private readonly ITelegramHandlerService _telegramService;

        public WebhookController(ITelegramHandlerService telegramService)
        {
            _telegramService = telegramService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            await _telegramService.HandleMessageAsync(update);
            return Ok();
        }
    }
}
