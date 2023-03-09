using TelegramBot;

var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

var services = new ServiceCollection();

services.AddCommonService(configuration);

var serviceProvider = services.BuildServiceProvider();


var telegramManager = serviceProvider.GetService<TelegramBotManager>();
await telegramManager.Start();



