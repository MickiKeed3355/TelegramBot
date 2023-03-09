using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data.Extensions;
using TelegramBot.Data.Model;
using TelegramBotDb.Model;

namespace TelegramBot
{
    public class TelegramBotManager
    {
        private readonly IDishesSaveService _dishesSaveService;
        private readonly IDishesGetService _dishesGetService;
        private readonly IUserSaveService _userSaveService;
        private readonly ComandTelegramService _comandTelegramService;
        private readonly CommonFileServices _commonFileServices;

        private readonly List<EnumTypeDish> TypeDishes = new List<EnumTypeDish>() { EnumTypeDish.First, EnumTypeDish.Second, EnumTypeDish.Meat };
        private List<string> ChatIds { get; set; } = new List<string>();
        public TelegramBotManager(
            ComandTelegramService comandTelegramService,
            IUserSaveService userSaveService,
            IDishesGetService dishesGetService,
            CommonFileServices commonFileServices,
            IDishesSaveService dishesSaveService)
        {
            _comandTelegramService = comandTelegramService;
            _userSaveService = userSaveService;
            _dishesGetService = dishesGetService;
            _commonFileServices = commonFileServices;
            _dishesSaveService = dishesSaveService;
        }

        private const string Token = "5968923050:AAGOriJ4dpIqiAmBi3yX9B_VvWsfVqzE1rI";
        public async Task Start()
        {
            ChatIds = await _userSaveService.GetAllChatid();
            var botClient = new TelegramBotClient(Token);

            using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                try
                {
                    var request = new TelegramRequest() { BotClient = botClient, CancellationToken = cancellationToken };
                    await _commonFileServices.WriteMessagesToFile(update, ChatIds, _userSaveService);
                    await CheckTypeMessage(request, update);
                    return;
                }
                catch (Exception ex)
                {
                    // ignore
                }
            }

            async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };
                Console.WriteLine(ErrorMessage);
                return;
            }
        }

        private async Task CheckTypeMessage(TelegramRequest request, Update update)
        {
            if (update.Type == UpdateType.Message)
            {
                await ExecuteActionMessage(request, update);
                return;
            }
            if (update.Type == UpdateType.CallbackQuery)
            {
                await ExecuteActionCallbackQuery(request, update);
                return;
            }
        }

        private async Task<bool> ExecuteActionMessage(TelegramRequest request, Update update)
        {
            var validationModel = await update.Validation(ChatIds, _userSaveService);

            if (!validationModel.IsValidate)
                return false;

            if (Guid.TryParse(validationModel.MessageModel.MessageText, out var code))
            {
                var dish = await _dishesSaveService.SaveDishByCode(new SaveDishByCodeByRequest() { Code = code, CreateChatId = validationModel.ChatId.ToString() });
                if (dish == null)
                {
                    await request.BotClient.SendTextMessageAsync(
                             chatId: validationModel.ChatId,
                             text: "Блюдо не найденно",
                             cancellationToken: request.CancellationToken);
                    return false;
                }

                var message = $"Тип блюда: {dish.TypeDish.ConvertEnumTypeDistToText()}" +
                    $"\n\n" +
                    $"Название блюда: {dish.Name}" +
                    $"\n\n" +
                    $"Рецепт блюда: {dish.Recipe}";
                await request.BotClient.SendTextMessageAsync(
                 validationModel.ChatId,
                 message,
                 cancellationToken: request.CancellationToken);
                return false;
            }

            if (await WorkToMenu(request, update, validationModel))
                return true;


            var userAction = await _userSaveService.GetUserActionMessageIdByChatId(validationModel.ChatId.ToString());

            var result = await SaveItearationAction(userAction, update, validationModel.ChatId.ToString(),
                validationModel.MessageModel.MessageText, request);

            if (result)
                return true;

            return true;
        }

        private async Task<bool> WorkToMenu(TelegramRequest request, Update update, ValidationResponse validationModel)
        {
            if (validationModel.MessageModel.MessageText == "Menu")
                return await _comandTelegramService.GetTotalMenu(request, validationModel.ChatId);

            if (await _comandTelegramService.ChooseDishes(request, update, validationModel.ChatId, validationModel.MessageModel.MessageText))
                return true;

            return false;
        }



        private async Task<bool> SaveItearationAction(GetMessageIfByUserActionResponse userAction, Update update, string chatId, string message, TelegramRequest request)
        {
            var iterationSaveDish = userAction?.IterationSaveDish ?? 0;
            switch (iterationSaveDish)
            {
                case 2:
                    if (message == "Отменить")
                        return await BreakCommand(userAction, update, chatId, request);

                    await _comandTelegramService.SaveActionNameDish(new(chatId, iterationSaveDish) { NameDish = message }, request);
                    return true;
                case 4:
                    if (message == "Отменить")
                        return await BreakCommand(userAction, update, chatId, request);

                    await _comandTelegramService.SaveActionNameDish(new(chatId) { RecipeDish = message }, request);
                    return true;
                case 6:
                    if (message == "Отменить")
                        return await BreakCommand(userAction, update, chatId, request);

                    if (message == "Сохранить новое блюдо")
                    {
                        await _comandTelegramService.SaveDish(chatId);
                        await _userSaveService.Remove(new RemoveActionRequest() { ChatId = chatId });
                        if (Enum.TryParse<EnumTypeDish>(userAction.TypeDish, out var typeDis))
                            return await _comandTelegramService.ChooseDishesByTypeDish(request, update, long.Parse(chatId), typeDis);
                        return true;
                    }
                    return true;
                default:
                    return await _comandTelegramService.GetTotalMenu(request, long.Parse(chatId));
            }
        }

        private async Task<bool> BreakCommand(GetMessageIfByUserActionResponse userAction, Update update, string chatId, TelegramRequest request)
        {
            await _userSaveService.Remove(new RemoveActionRequest() { ChatId = chatId });

            if (Enum.TryParse<EnumTypeDish>(userAction.TypeDish, out var typeDis))
                await _comandTelegramService.ChooseDishesByTypeDish(request, update, long.Parse(chatId), typeDis);

            return true;

        }
        private async Task ExecuteActionCallbackQuery(TelegramRequest request, Update update)
        {
            var validationModel = await update.Validation(ChatIds, _userSaveService);

            if (!validationModel.IsValidate)
                return;


            if (int.TryParse(update.CallbackQuery?.Data, out var idDish))
            {
                var userAction = await _userSaveService.GetUserActionMessageIdByChatId(validationModel.ChatId.ToString());
                if (userAction != null)
                {
                    if (userAction.NameDish == "Поделиться")
                    {
                        var guid = await _dishesGetService.GetDishCodeById(idDish);
                        if (guid == null)
                        {
                            await request.BotClient.SendTextMessageAsync(
                             chatId: validationModel.ChatId,
                             text: "Блюдо не найденно",
                             cancellationToken: request.CancellationToken);
                            return;
                        }

                        var message = $"{guid}";
                        message = message.Replace("-", @"\-");

                        await request.BotClient.SendTextMessageAsync(
                              chatId: validationModel.ChatId,
                              text: message,
                              parseMode: ParseMode.MarkdownV2,
                              cancellationToken: request.CancellationToken);
                        await _comandTelegramService.GetTotalMenu(request, validationModel.ChatId);
                        await _userSaveService.Remove(new RemoveActionRequest() { ChatId = validationModel.ChatId.ToString() });

                        return;
                    }

                    var response = await _dishesSaveService.RemoveDish(new RemoveDishRequest()
                    {
                        DishId = idDish,
                        ChatId = validationModel.ChatId.ToString()
                    });

                    if (!response.Success)
                    {
                        await request.BotClient.SendTextMessageAsync(
                              chatId: validationModel.ChatId,
                              text: response.ErrorMessgae,
                              cancellationToken: request.CancellationToken);
                        return;
                    }


                    if (Enum.TryParse<EnumTypeDish>(userAction.TypeDish, out var typeDishRemove))
                        await _comandTelegramService.ChooseDishesByTypeDish(request, update, validationModel.ChatId, typeDishRemove);

                    await _userSaveService.Remove(new RemoveActionRequest() { ChatId = validationModel.ChatId.ToString() });
                    return;
                }

                var dishRecipe = await _dishesGetService.GetDishRecipeById(idDish);

                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
{
    new KeyboardButton[] { "Первое 🥘", "Второе 🍔", "Готовка мяса 🍖" },
})
                {
                    ResizeKeyboard = true
                };

                await request.BotClient.SendTextMessageAsync(
                   validationModel.ChatId,
                    dishRecipe,
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: request.CancellationToken);
                return;
            }

            var commandParts = update.CallbackQuery.Data.Split('\n');
            var typeIsParse = Enum.TryParse<EnumTypeDish>(commandParts[0], out var typeDish);

            if (typeIsParse && TypeDishes.Contains(typeDish))
            {

                switch (commandParts[1])
                {
                    case "Удалить":
                        var requestAction = new SaveActionRequest(validationModel.ChatId.ToString(), 0)
                        {
                            TypeDish = commandParts[0]
                        };

                    await    _comandTelegramService.SaveActionRemove(requestAction, request, validationModel.ChatId).ConfigureAwait(false);
                        return;

                    case "Сохрнаить":
                     await   _comandTelegramService.SaveActionTypeDish(new(validationModel.ChatId.ToString(), 2)
                        {
                            MessageId = validationModel.MessageModel.Message.MessageId.ToString(),
                            TypeDish = typeDish.ToString()
                        }, request).ConfigureAwait(false);
                        return;
                    case "Поделиться":
                  await    _comandTelegramService.SaveActionShare(new(validationModel.ChatId.ToString(), 2)
                        {
                            MessageId = validationModel.MessageModel.Message.MessageId.ToString(),
                            NameDish = "Поделиться"
                        }, request, validationModel.ChatId).ConfigureAwait(false);
                        return;
                    default: break;
                }



            }

        }
    }
}