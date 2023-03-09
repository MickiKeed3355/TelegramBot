namespace TelegramBot.Service.Test
{
    public static class DependencyResolver
    {
        private static IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        private static IServiceProvider? _serviceProvider = null;

        public static IServiceProvider ServiceProvider { get { return _serviceProvider ?? InitServiceProvider(); } }

        private static IServiceProvider InitServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddCommonService(Configuration);

            return services.BuildServiceProvider();
        }

        public static T? GetService<T>() where T : class => ServiceProvider.GetService<T>();
    }
}
