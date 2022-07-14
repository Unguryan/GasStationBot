using GasStationBot.Application.Services;
using GasStationBot.Application.Services.Telegram;
using GasStationBot.Domain.Entities;
using GasStationBot.TelegramBot.Commands;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GasStationBot.TelegramBot.Core
{
    public class TelegramHandlerService : ITelegramHandlerService
    {
        private readonly ITelegramMessageMediator _mediator;

        private readonly ITelegramUserStateService _userStateService;

        private readonly IUserService _userService;

        private readonly ITelegramCommandFactory _commandFactory;

        //private readonly ITelegramBotClient _botClient;

        public TelegramHandlerService(ITelegramMessageMediator mediator,
                                      ITelegramUserStateService userStateService,
                                      IUserService userService,
                                      ITelegramCommandFactory commandFactory)
        {
            _mediator = mediator;
            _userStateService = userStateService;
            _userService = userService;
            _commandFactory = commandFactory;
            //_botClient = botClient;
        }

        public async Task HandleMessageAsync(Update update)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                UpdateType.Message => BotOnMessageReceived(update.Message!),
                //UpdateType.EditedMessage => BotOnMessageReceived(update.EditedMessage!),
                //UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery!),
                //UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery!),
                //UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult!),
                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception);
            }
        }

        private async Task BotOnMessageReceived(Message message)
        {
            var userId = message.Chat.Id.ToString();
            var userMessage = message.Text!;
            var userState = _userStateService.GetUserState(userId);

            //TODO:Fix this
            UserState? updatedUserState = null;

            //Case, if new user
            if (userState == null)
            {
                updatedUserState = await _mediator.Send(new StartMenuCommand(userId, userMessage));
                _userStateService.SetUserState(userId, updatedUserState.Value);
                
                await CheckOrAddUser(message.Chat.Id.ToString(), message.Chat.FirstName!);

                var baseMenuCommand = _commandFactory.TryToCreateEmptyCommandByUserState(updatedUserState.Value, userId);
                await _mediator.Send(baseMenuCommand);
                return;
            }

            //Check more priority command from telegram,
            var parseResult = _commandFactory.TryToCreateCommandByTelegramCommand(userMessage, userId);
            if (parseResult != null)
            {
                updatedUserState = await _mediator.Send(parseResult);
                _userStateService.SetUserState(userId, updatedUserState.Value);
            }
            else
            {
                parseResult = _commandFactory.TryToCreateCommandByUserState(userState.Value, userMessage, userId);
                if (parseResult == null)
                {
                    throw new ArgumentException("Command does not exist.");
                }

                updatedUserState = await _mediator.Send(parseResult);
                _userStateService.SetUserState(userId, updatedUserState.Value);
            }

            //TODO:Add null check
            var nextCommand = _commandFactory.TryToCreateEmptyCommandByUserState(updatedUserState.Value, userId);
            await _mediator.Send(nextCommand);

            //1.Get User State
            //2.Parse Message by command "/start"
            //3.Parse current userState
            //4.Send Command to mediator by state 
            //5.Update userState
            //6.Send Message from new state

            //Handle Command User
            // "/start" - Send Start
            // "/about" - Send About
            // "/menu" - Send Base Menu


        }

        private async Task CheckOrAddUser(string userId, string firstName)
        {
            var user = await _userService.GetUserById(userId);

            if(user == null)
            {
                await _userService.AddUser(new Domain.Entities.User() { Id = userId, Name = firstName, GasStations = new List<GasStation>() });
            }
        }

        //private async Task BotOnMessageReceived(Message message)
        //{
        //    //_logger.LogInformation("Receive message type: {MessageType}", message.Type);
        //    if (message.Type != MessageType.Text)
        //        return;

        //    var action = message.Text!.Split(' ')[0] switch
        //    {
        //        "/inline" => SendInlineKeyboard(_botClient, message),
        //        "/keyboard" => SendReplyKeyboard(_botClient, message),
        //        "/remove" => RemoveKeyboard(_botClient, message),
        //        "/photo" => SendFile(_botClient, message),
        //        "/request" => RequestContactAndLocation(_botClient, message),
        //        _ => Usage(_botClient, message)
        //    };
        //    Message sentMessage = await action;
        //    //_logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

        //    // Send inline keyboard
        //    // You can process responses in BotOnCallbackQueryReceived handler
        //    static async Task<Message> SendInlineKeyboard(ITelegramBotClient bot, Message message)
        //    {
        //        await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

        //        // Simulate longer running task
        //        await Task.Delay(500);

        //        InlineKeyboardMarkup inlineKeyboard = new(
        //            new[]
        //            {
        //            // first row
        //            new []
        //            {
        //                InlineKeyboardButton.WithCallbackData("1.1", "11"),
        //                InlineKeyboardButton.WithCallbackData("1.2", "12"),
        //            },
        //            // second row
        //            new []
        //            {
        //                InlineKeyboardButton.WithCallbackData("2.1", "21"),
        //                InlineKeyboardButton.WithCallbackData("2.2", "22"),
        //            },
        //            });

        //        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
        //                                              text: "Choose",
        //                                              replyMarkup: inlineKeyboard);
        //    }

        //    static async Task<Message> SendReplyKeyboard(ITelegramBotClient bot, Message message)
        //    {
        //        ReplyKeyboardMarkup replyKeyboardMarkup = new(
        //            new[]
        //            {
        //                new KeyboardButton[] { "1.1", "1.2" },
        //                new KeyboardButton[] { "2.1", "2.2" },
        //            })
        //        {
        //            ResizeKeyboard = true
        //        };

        //        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
        //                                              text: "Choose",
        //                                              replyMarkup: replyKeyboardMarkup);
        //    }

        //    static async Task<Message> RemoveKeyboard(ITelegramBotClient bot, Message message)
        //    {
        //        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
        //                                              text: "Removing keyboard",
        //                                              replyMarkup: new ReplyKeyboardRemove());
        //    }

        //    static async Task<Message> SendFile(ITelegramBotClient bot, Message message)
        //    {
        //        await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

        //        const string filePath = @"Files/tux.png";
        //        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

        //        return await bot.SendPhotoAsync(chatId: message.Chat.Id,
        //                                        photo: new InputOnlineFile(fileStream, fileName),
        //                                        caption: "Nice Picture");
        //    }

        //    static async Task<Message> RequestContactAndLocation(ITelegramBotClient bot, Message message)
        //    {
        //        ReplyKeyboardMarkup RequestReplyKeyboard = new(
        //            new[]
        //            {
        //            KeyboardButton.WithRequestLocation("Location"),
        //            KeyboardButton.WithRequestContact("Contact"),
        //            });

        //        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
        //                                              text: "Who or Where are you?",
        //                                              replyMarkup: RequestReplyKeyboard);
        //    }

        //    static async Task<Message> Usage(ITelegramBotClient bot, Message message)
        //    {
        //        const string usage = "Usage:\n" +
        //                             "/inline   - send inline keyboard\n" +
        //                             "/keyboard - send custom keyboard\n" +
        //                             "/remove   - remove custom keyboard\n" +
        //                             "/photo    - send a photo\n" +
        //                             "/request  - request location or contact";

        //        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
        //                                              text: usage,
        //                                              replyMarkup: new ReplyKeyboardRemove());
        //    }
        //}

        // Process Inline Keyboard callback data
        //private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        //{
        //    await _botClient.AnswerCallbackQueryAsync(
        //        callbackQueryId: callbackQuery.Id,
        //        text: $"Received {callbackQuery.Data}");

        //    await _botClient.SendTextMessageAsync(
        //        chatId: callbackQuery.Message!.Chat.Id,
        //        text: $"Received {callbackQuery.Data}");
        //}

        //#region Inline Mode

        //private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)
        //{
        //    //_logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        //    InlineQueryResult[] results = {
        //    // displayed result
        //    new InlineQueryResultArticle(
        //        id: "3",
        //        title: "TgBots",
        //        inputMessageContent: new InputTextMessageContent(
        //            "hello"
        //        )
        //    )
        //};

        //    await _botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
        //                                            results: results,
        //                                            isPersonal: true,
        //                                            cacheTime: 0);
        //}

        //private Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
        //{
        //    //_logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);
        //    return Task.CompletedTask;
        //}

        //#endregion

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            //_logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }

        private Task HandleErrorAsync(Exception exception)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            //_logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
            return Task.CompletedTask;
        }


    }
}
