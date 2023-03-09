using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.IService.Model
{
    public class RemoveDishRequest
    {
        public long DishId { get; set; }
        public string ChatId { get; set; }
    }
}
