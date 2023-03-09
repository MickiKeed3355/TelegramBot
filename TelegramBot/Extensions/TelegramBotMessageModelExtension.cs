namespace TelegramBot.Extensions
{
    public static class TelegramBotMessageModelExtension
    {
        public static string GetUserName(this Message message)
        {
            return message?.From?.FirstName + " " + message?.From?.LastName;
        }
    }
}
