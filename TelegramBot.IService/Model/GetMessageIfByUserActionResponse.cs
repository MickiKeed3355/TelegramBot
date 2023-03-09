using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.IService.Model
{
    public class GetMessageIfByUserActionResponse
    {
        public string? MessageId { get; set; }
        public uint IterationSaveDish { get; set; }
        public string? TypeDish { get; set; }
        public string? NameDish { get; set; }
    }
}
