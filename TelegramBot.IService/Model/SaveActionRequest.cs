namespace TelegramBot.IService.Model
{
    public class SaveActionRequest
    {
        public string? TypeDish { get; set; }
        public string? NameDish { get; set; }
        public string? RecipeDish { get; set; }
        public string ChatId { get; set; }
        public string? MessageId { get; set; }
        public uint? Iteration { get; set; }
        public SaveActionRequest(string chatId, uint? iteration=null)
        {
            ChatId = chatId;
            Iteration = iteration;
        }
    }
}
