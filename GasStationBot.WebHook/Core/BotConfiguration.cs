namespace GasStationBot.WebHook.Core
{
    public class BotConfiguration
    {
        public string BotToken { get; set; } = default!;
        public string HostAddress { get; init; } = default!;
    }
}
