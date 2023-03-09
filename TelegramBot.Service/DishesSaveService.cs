using Microsoft.EntityFrameworkCore;
using TelegramBot.Data.Model;
using TelegramBot.IService;
using TelegramBot.IService.CommonModel;
using TelegramBot.IService.Model;
using TelegramBotDb;
using TelegramBotDb.Model;

namespace TelegramBot.Service
{
    public class DishesSaveService: IDishesSaveService
    {
        private readonly DbContextOptions<DataContext> _dbContextOptions;

        public DishesSaveService(DbContextOptions<DataContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }
        public async Task<bool> SaveDish(SaveDishRequest request)
        {
            try
            {
                await using var db = new DataContext(_dbContextOptions);
                var dish = new Dish()
                {
                    Name = request.Name,
                    Recipe = request.Recipe,
                    TypeDish = request.TypeDish,
                    Code=Guid.NewGuid(),
                    DishUserAccess =new List<DishUserAccess>() { new DishUserAccess() { CreateChatId = request.CreateChatId } },
                    DishSetting =new DishSetting() {IsPrivate=true}         
                };
                await db.Dishes.AddAsync(dish);


                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<Dish?> SaveDishByCode(SaveDishByCodeByRequest request)
        {
            await using var db = new DataContext(_dbContextOptions);

            var dish = await db.Dishes.Where(x => x.Code == request.Code).FirstOrDefaultAsync();

            if (dish == null)
                return null;

            var accessDish = new DishUserAccess() { CreateChatId = request.CreateChatId, Dish = dish };

            await db.DishUserAccess.AddAsync(accessDish);
            await db.SaveChangesAsync();

            return dish;
        }
        public async Task<BaseResponse> RemoveDish(RemoveDishRequest request)
        {
            try
            {
                await using var db = new DataContext(_dbContextOptions);

                var dish = await db.Dishes.FirstOrDefaultAsync(x => x.Id == request.DishId);

                if (dish == null)
                    return new BaseResponse("Нет такого блюда");

                var accesToDish = await db.DishUserAccess.FirstOrDefaultAsync(x => x.DishId == dish.Id && x.CreateChatId == request.ChatId);
                if (accesToDish==null)
                    return new BaseResponse("Блюдо не принадлежит вам");

                db.DishUserAccess.Remove(accesToDish);
                await db.SaveChangesAsync();
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                return new BaseResponse("Нет такого блюда");
            }
        }

    }
}
