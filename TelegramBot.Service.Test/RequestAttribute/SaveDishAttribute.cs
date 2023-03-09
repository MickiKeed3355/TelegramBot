using System.Reflection;

namespace TelegramBot.Service.Test.RequestAttribute
{
    public class SaveDishAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            yield return new object[] {new SaveDishRequest()
            {
                Name = "Суп",
                Recipe = "Курица",
                CreateChatId = "1195895163",
                TypeDish = EnumTypeDish.First
            }, true};
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return "Сохранить блюдо";
        }
    }
}
