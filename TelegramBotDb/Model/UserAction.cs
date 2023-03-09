namespace TelegramBot.Data.Model
{
    public class UserAction
    {
        public uint Id { get; set; }
        public string? ChatId { get; set; }
        public string? TypeDish { get; set; }
        public string? NameDish { get; set; }
        public string? RecipeDish { get; set; }
        public string? MessageId {get; set;}
        public uint IterationSaveDish { get; set; }
    }
}
