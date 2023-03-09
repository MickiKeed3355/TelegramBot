using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotDb.Model;

namespace TelegramBot.Data.Model
{
    public class DishSetting
    {
        public long DishId { get; set; }
        public Dish Dish { get; set; }
        public bool IsPrivate { get; set; }
    }
}
