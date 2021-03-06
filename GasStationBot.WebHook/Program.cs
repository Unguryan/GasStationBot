using GasStationBot.Infrastructure;
using GasStationBot.Infrastructure.DB;
using GasStationBot.WebHook.Core;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
botConfig.BotToken = builder.Configuration["BotToken"];
builder.Services.AddHostedService<ConfigureWebhook>();

builder.Services.AddHttpClient("tgwebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botConfig.BotToken, httpClient));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers().AddNewtonsoftJson();



var app = builder.Build();

app.UseRouting();
app.UseCors();

app.UseEndpoints(endpoints =>
{
    var token = botConfig.BotToken;
    endpoints.MapControllerRoute(name: "tgwebhook",
                                 pattern: $"bot/{token}",
                                 new { controller = "Webhook", action = "Post" });
    endpoints.MapControllers();
});

//just to fix
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GasStationContext>();
        context.Database.EnsureCreated();
        context.SaveChanges();
    }
    catch
    {
    }
}

app.Run();
