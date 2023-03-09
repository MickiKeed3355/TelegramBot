using Microsoft.EntityFrameworkCore;
using TelegramBot.Data.Model;
using TelegramBot.IService.Model;
using TelegramBot.Service.Interface;
using TelegramBot.Service.Model;
using TelegramBotDb;

namespace TelegramBot.Service
{
    public class UserSaveService : IUserSaveService
    {
        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public UserSaveService(DbContextOptions<DataContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<List<string>> GetAllChatid()
        {
            await using var db = new DataContext(_dbContextOptions);

            return await db.Users.Select(x => x.ChatId).ToListAsync();
        }

        public Task<string> GetRecipeOfDish()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save(SaveRequest request)
        {
            await using var db = new DataContext(_dbContextOptions);
            await db.Users.AddAsync(new TelegramBotDb.Model.User()
            {
                ChatId = request.ChatId,
                Name = request.Name
            });
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveAction(SaveActionRequest request)
        {
            await using var db = new DataContext(_dbContextOptions);
            var userAction = await db.UserActions.FirstOrDefaultAsync(x=>x.ChatId==request.ChatId)??new UserAction();
         
            SetUserActionData(request, userAction);

            if (userAction.Id == 0)
                await db.UserActions.AddAsync(userAction);

            await db.SaveChangesAsync();
            return true;
        }

        private void SetUserActionData(SaveActionRequest request, UserAction userAction)
        {
            if (request.NameDish != null)
                userAction.NameDish = request.NameDish;
            if (request.TypeDish != null)
                userAction.TypeDish = request.TypeDish;
            if (request.RecipeDish != null)
                userAction.RecipeDish = request.RecipeDish;
            if (request.ChatId != null)
                userAction.ChatId = request.ChatId;
            if (request.MessageId != null)
                userAction.MessageId = request.MessageId;

            userAction.IterationSaveDish= request.Iteration.HasValue? request.Iteration.Value : userAction.IterationSaveDish + 2;
        }

        public async Task<GetMessageIfByUserActionResponse?> GetUserActionMessageIdByChatId(string chatId)
        {
            await using var db = new DataContext(_dbContextOptions);
            return await db.UserActions
                .Where(x => x.ChatId == chatId)
                .Select(x =>new GetMessageIfByUserActionResponse()
                {
                    MessageId = x.MessageId,
                    IterationSaveDish = x.IterationSaveDish,
                    TypeDish = x.TypeDish,
                    NameDish= x.NameDish,
                }).FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<UserAction?> GetUserActionByChatId(string chatId)
        {
            await using var db = new DataContext(_dbContextOptions);
            return await db.UserActions
                .Where(x => x.ChatId == chatId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<bool> Remove(RemoveActionRequest request)
        {
            await using var db = new DataContext(_dbContextOptions);
            var actions = await db.UserActions.Where(x => x.ChatId == request.ChatId).ToArrayAsync();
            if (!actions.Any())
                return true;
            db.RemoveRange(actions);
            await db.SaveChangesAsync();
            return true;
        }

    }
}