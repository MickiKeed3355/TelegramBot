namespace TelegramBot.HelperModel
{
    public class MessageModel
    {
        public Message Message { get; set; }
        public string MessageText { get; set; }
      
        public MessageModel(Message message, string messageText)
        {
            Message = message;
            MessageText = messageText;
        }
    }
}
