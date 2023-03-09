namespace TelegramBot.Services
{
    public class CommonFileServices
    {
        public async Task WriteMessagesToFile(Update Update, List<string> chatIds, IUserSaveService userSaveService)
        {
            var validationModel = await Update.Validation(chatIds, userSaveService);

            if (!validationModel.IsValidate)
                return;

            var userName = validationModel.MessageModel?.Message.GetUserName();
            var messageToFile = $"Id:{validationModel.ChatId}, UserName:{userName}, UserMessage:{validationModel.MessageModel?.MessageText}, Date:{DateTime.UtcNow}";

            //await SaveMessageToFile(messageToFile, userName);
        }

        public async Task SaveMessageToFile(string message, string userName)
        {
            await CheckIsCreateFolder();

            var path = @$"UserMessages\{userName}.txt";
            message += "\n\n";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                await writer.WriteLineAsync(message);
            }
        }

        private async Task CheckIsCreateFolder()
        {
            string pathDirectory = Environment.CurrentDirectory;
            string subpath = @"UserMessages";
            DirectoryInfo dirInfo = new DirectoryInfo(pathDirectory);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            dirInfo.CreateSubdirectory(subpath);
        }
    }
}
