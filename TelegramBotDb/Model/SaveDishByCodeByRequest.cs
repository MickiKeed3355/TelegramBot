namespace TelegramBot.Data.Model
{
    public class SaveDishByCodeByRequest
    {
        public Guid Code { get; set; }
        public string CreateChatId { get; set; }
    }
}
