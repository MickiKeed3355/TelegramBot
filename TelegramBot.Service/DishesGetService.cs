using Microsoft.EntityFrameworkCore;
using TelegramBot.Service.Interface;
using TelegramBot.Service.Model;
using TelegramBotDb;
using TelegramBotDb.Model;

namespace TelegramBot.Service
{
    public class DishesGetService : IDishesGetService
    {
        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public DishesGetService(DbContextOptions<DataContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<List<GetAllFirstDishesResponse>> GetAllDishesByTypeDish(EnumTypeDish typeDish, string chatId)
        {
            try
            {
                await using var db = new DataContext(_dbContextOptions);

                var dishes = await db.Dishes
                    .Where(x => x.TypeDish == typeDish&&
                    (  x.DishSetting == null || !x.DishSetting.IsPrivate || x.DishUserAccess.Any(s=>s.CreateChatId==chatId)))
                    .Select(x =>                        
                        new GetAllFirstDishesResponse()
                        {
                            Id = x.Id,
                            Name = (x.DishSetting == null || !x.DishSetting.IsPrivate)?x.Name : x.Name + " (your)",
                        }).ToListAsync();
                return dishes;
            }
            catch (Exception ex)
            {
                return new List<GetAllFirstDishesResponse>();
            }
        }

        public async Task<string> GetDishRecipeById(long id)
        {
            await using var db = new DataContext(_dbContextOptions);

            var dish = await db.Dishes.Where(x => x.Id == id).Select(x => new { x.Recipe }).FirstOrDefaultAsync();

            if (dish == null)
                return "";

            return dish.Recipe;
        }
        public async Task<Guid?> GetDishCodeById(long id)
        {
            await using var db = new DataContext(_dbContextOptions);

            var dish = await db.Dishes.Where(x => x.Id == id).Select(x => new {x.Code}).FirstOrDefaultAsync();

            if (dish == null)
                return null;

            return dish.Code;
        }

       
    }
}
