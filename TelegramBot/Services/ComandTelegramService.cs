using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.HelperModel;
using TelegramBotDb.Model;

namespace TelegramBot.Services
{
    public class ComandTelegramService
    {
        private readonly IDishesGetService _dishesGetService;
        private readonly IDishesSaveService _dishesSaveService;
        private readonly IUserSaveService _userSaveService;
        public ComandTelegramService(
            IDishesGetService dishesGetService,
            IDishesSaveService dishesSaveService,
            IUserSaveService userSaveService)
        {
            _dishesGetService = dishesGetService;
            _dishesSaveService = dishesSaveService;
            _userSaveService = userSaveService;
        }

        public async Task<bool> GetTotalMenu(TelegramRequest request, long chatId)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
{
    new KeyboardButton[] { "Первое 🥘", "Второе 🍔", "Готовка мяса 🍖" },
})
            {
                ResizeKeyboard = true
            };

            await request.BotClient.SendTextMessageAsync(
               chatId,
              "Выберите тип блюда в меню",
               replyMarkup: replyKeyboardMarkup,
               cancellationToken: request.CancellationToken);

            return true;
        }

        public async Task GetButtonMenu(TelegramRequest request, long chatId)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
{
    new KeyboardButton[] { "Menu" },
})
            {
                ResizeKeyboard = true
            };

            await request.BotClient.SendTextMessageAsync(
               chatId,
              "Нажмите на кнопку Menu",
               replyMarkup: replyKeyboardMarkup,
               cancellationToken: request.CancellationToken);
        }

        public async Task SaveActionTypeDish(SaveActionRequest requestAction, TelegramRequest request)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                  new KeyboardButton[] { "Отменить"},
                 })
            {
                ResizeKeyboard = true
            };

            await _userSaveService.SaveAction(requestAction);
         
            await request.BotClient.SendTextMessageAsync(
               chatId: requestAction.ChatId,
              "Введите название блюда",
              replyMarkup: replyKeyboardMarkup,
               cancellationToken: request.CancellationToken);
        }
        
        public async Task SaveActionRemove(SaveActionRequest requestAction, TelegramRequest request, long chatId)
        {       
            await _userSaveService.SaveAction(requestAction); 
            await request.BotClient.SendTextMessageAsync(
                 chatId,
                 "Выбирете блюдо из списка, которое хотите удалить",
                 cancellationToken: request.CancellationToken);
        }
        public async Task SaveActionShare(SaveActionRequest requestAction, TelegramRequest request, long chatId)
        {
            await _userSaveService.SaveAction(requestAction);
            await request.BotClient.SendTextMessageAsync(
                 chatId,
                 "Выбирете блюдо из списка, которым хотите поделиться",
                 cancellationToken: request.CancellationToken);
        }
        public async Task SaveActionNameDish(SaveActionRequest requestAction,  TelegramRequest request)
        {
            var  command= new List<KeyboardButton> { "Отменить" };
           
            string? messageToChat;
            if (requestAction.Iteration == 2)
                messageToChat = "Введите рецепт блюда";
            else
            {
                messageToChat = "Выберите комманду";
                command.Add("Сохранить новое блюдо");
            }

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new List<List<KeyboardButton>>() { command })
            {
                ResizeKeyboard = true
            };
            requestAction.Iteration = null;
            await _userSaveService.SaveAction(requestAction);
            await request.BotClient.SendTextMessageAsync(
             chatId: requestAction.ChatId,
              messageToChat,
              replyMarkup: replyKeyboardMarkup,
               cancellationToken: request.CancellationToken);
        }

        public async Task SaveDish(string chatId)
        {
            var userAction = await _userSaveService.GetUserActionByChatId(chatId);
            if (userAction == null)
                return;

            var typeDish = Enum.Parse<EnumTypeDish>(userAction.TypeDish);
            await _dishesSaveService.SaveDish(new SaveDishRequest() { Name = userAction.NameDish, CreateChatId = chatId, Recipe = userAction.RecipeDish, TypeDish = typeDish });
        }

        public async Task GetDishes(TelegramRequest request, long chatId, EnumTypeDish typeDish, string? languageCode)
        {
            var InlineKeyboardButtons = new List<List<InlineKeyboardButton>>();

        
            var firstDishes = await _dishesGetService.GetAllDishesByTypeDish(typeDish, chatId.ToString());
            foreach (var dishes in firstDishes)
            {
                InlineKeyboardButtons.Add(new List<InlineKeyboardButton>()
                             {
                       InlineKeyboardButton.WithCallbackData(dishes.Name, dishes.Id.ToString())

                             });
            }

            var commandLnSave = "Сохрнаить";
            var commandLnRemove = "Удалить";
            var commandLnShare= "Поделиться";

            InlineKeyboardButtons.Add(new List<InlineKeyboardButton>()
                             {
                       InlineKeyboardButton.WithCallbackData(commandLnSave, typeDish.ToString()+$"\n{commandLnSave}"),
                       InlineKeyboardButton.WithCallbackData(commandLnRemove, typeDish.ToString()+$"\n{commandLnRemove}"),
                        InlineKeyboardButton.WithCallbackData(commandLnShare, typeDish.ToString()+$"\n{commandLnShare}")
                             });

            InlineKeyboardMarkup inlineKeyboard = new(InlineKeyboardButtons);

            await request.BotClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выберите блюдо",
                replyMarkup: inlineKeyboard,
                cancellationToken: request.CancellationToken);
        }

        public async Task<bool> ChooseDishes(TelegramRequest request, Update update, long chatId, string text)
        {
            var ln = update.Message?.From?.LanguageCode;

            switch (text)
            {
                case "Первое \U0001f958":
                    await GetDishes(request, chatId, EnumTypeDish.First, ln);
                    return true;
                case "Второе 🍔":
                    await GetDishes(request, chatId, EnumTypeDish.Second, ln);
                    return true;
                case "Готовка мяса 🍖":
                    await GetDishes(request, chatId, EnumTypeDish.Meat, ln);
                    return true;
                default: return false;
            }
        }
        public async Task<bool> ChooseDishesByTypeDish(TelegramRequest request, Update update, long chatId, EnumTypeDish typeDish)
        {
            var ln = update.Message?.From?.LanguageCode;

            switch (typeDish)
            {
                case EnumTypeDish.First:
                    await GetDishes(request, chatId, EnumTypeDish.First, ln);
                    return true;
                case EnumTypeDish.Second:
                    await GetDishes(request, chatId, EnumTypeDish.Second, ln);
                    return true;
                case EnumTypeDish.Meat:
                    await GetDishes(request, chatId, EnumTypeDish.Meat, ln);
                    return true;
                default: return false;
            }
        }


    }
}