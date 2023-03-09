using TelegramBot.Data.Model;
using TelegramBot.IService.Model;
using TelegramBot.Service.Model;

namespace TelegramBot.Service.Interface
{
    public interface IUserSaveService
    {
        Task<List<string>> GetAllChatid();
        Task<string> GetRecipeOfDish();
        Task<bool> Save(SaveRequest request);
        Task<bool> SaveAction(SaveActionRequest request);
        Task<bool> Remove(RemoveActionRequest request);
        Task<GetMessageIfByUserActionResponse?> GetUserActionMessageIdByChatId(string chatId);
        Task<UserAction?> GetUserActionByChatId(string chatId);
    }
}
