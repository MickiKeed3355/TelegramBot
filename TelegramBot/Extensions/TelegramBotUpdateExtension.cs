

namespace TelegramBot.Extensions
{
    public static class TelegramBotUpdateExtension
    {
        public static async Task<ValidationResponse> Validation(this Update update, List<string> chatIds, IUserSaveService userSaveService)
        {
            var message = update.Message is { }
               ? update.Message
               : update.CallbackQuery?.Message is { }
               ? update?.CallbackQuery?.Message
               : null;

            if (message == null)
                return new ValidationResponse(false);

            if (message.Text is not { } messageText)
                return new ValidationResponse(false);

            var chatId = message.Chat.Id.ToString();

            if (!chatIds.Contains(chatId))
            {
                var userName = message?.From?.FirstName + " " + message?.From?.LastName;
                await userSaveService.Save(new SaveRequest() { ChatId = chatId, Name = userName });
                chatIds.Add(chatId);
            }

            return new ValidationResponse(new MessageModel(message, messageText), long.Parse(chatId), true); 
        }
    }
}