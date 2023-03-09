namespace TelegramBot.Extensions
{
    public static class StartupExtension
    {
        public static void AddCommonService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);

            services.AddScoped<ComandTelegramService>();
            services.AddSingleton<TelegramBotManager>();
            services.AddSingleton<CommonFileServices>();
            services.AddTransient<IUserSaveService, UserSaveService>();
            services.AddTransient<IDishesGetService, DishesGetService>();
            services.AddTransient<IDishesSaveService, DishesSaveService>();
        }

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<DataContext>(options =>
                            options.UseSqlServer(connectionString));
        }
    }
}
