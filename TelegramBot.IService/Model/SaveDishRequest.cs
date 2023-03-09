using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotDb.Model;

namespace TelegramBot.IService.Model
{
    public class SaveDishRequest
    {
        public string Name { get; set; }
        public string Recipe { get; set; }
        public EnumTypeDish TypeDish { get; set; }
        public string CreateChatId { get; set; }
    }
}