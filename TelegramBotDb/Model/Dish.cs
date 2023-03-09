using TelegramBot.Data.Model;

namespace TelegramBotDb.Model
{
    public class Dish
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Recipe { get; set; }
        public Guid? Code { get; set; }
        public EnumTypeDish TypeDish { get; set; }
        public List<DishUserAccess> DishUserAccess { get; set; }
        public DishSetting DishSetting { get; set; }
    }

    public enum EnumTypeDish
    {
        First = 0,
        Second = 1,
        Meat = 2,
    }
}
