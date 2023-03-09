using TelegramBot.Service.Model;
using TelegramBotDb.Model;

namespace TelegramBot.Service.Interface
{
    public interface IDishesGetService
    {
        Task<List<GetAllFirstDishesResponse>> GetAllDishesByTypeDish(EnumTypeDish typeDish, string chatId);

        Task<string> GetDishRecipeById(long id);
        Task<Guid?> GetDishCodeById(long id);
    }
}
