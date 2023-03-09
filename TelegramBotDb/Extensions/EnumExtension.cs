using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotDb.Model;

namespace TelegramBot.Data.Extensions
{
    public static class EnumExtension
    {
        public static string ConvertEnumTypeDistToText(this EnumTypeDish typeDish)
        {
            switch (typeDish)
            {
                case EnumTypeDish.First: return "Первое 🥘";
                case EnumTypeDish.Second: return "Второе 🍔";
                case EnumTypeDish.Meat: return "Готовка мяса 🍖";
                default: return "Неизвестен";
            }
        }
    }
}
