using TelegramBot.Data.Model;
using TelegramBot.IService.CommonModel;
using TelegramBot.IService.Model;
using TelegramBotDb.Model;

namespace TelegramBot.IService
{
    public interface IDishesSaveService
    {
        Task<bool> SaveDish(SaveDishRequest request);
        Task<BaseResponse> RemoveDish(RemoveDishRequest request);
        Task<Dish?> SaveDishByCode(SaveDishByCodeByRequest request);
    }
}
