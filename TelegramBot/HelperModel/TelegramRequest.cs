

namespace TelegramBot.HelperModel
{
    public class TelegramRequest
    {
        public ITelegramBotClient BotClient { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
