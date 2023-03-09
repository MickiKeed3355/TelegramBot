using TelegramBotDb.Model;

namespace TelegramBot.Data.Model
{
    public class DishUserAccess
    {
        public long Id { get; set; }
        public long DishId { get; set; }
        public Dish Dish { get; set; }
        public string CreateChatId { get; set; }
    }
}
