using Microsoft.EntityFrameworkCore;
using TelegramBot.Data.Model;
using TelegramBotDb.Model;

namespace TelegramBotDb
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishUserAccess> DishUserAccess { get; set; }
        public DbSet<UserAction> UserActions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(t => t.Id);
            modelBuilder.Entity<Dish>().HasKey(t => t.Id);
            modelBuilder.Entity<DishUserAccess>().HasKey(t => t.Id);
            modelBuilder.Entity<DishUserAccess>()
                .HasOne(t => t.Dish)
                .WithMany(t=>t.DishUserAccess)
                .HasForeignKey(x=>x.DishId);
            modelBuilder.Entity<DishSetting>().HasKey(t => t.DishId);
            modelBuilder.Entity<DishSetting>()
                .HasOne(t => t.Dish)
                .WithOne(t => t.DishSetting)
                .HasForeignKey<DishSetting>(x => x.DishId);
        }
    }
}