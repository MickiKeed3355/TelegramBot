namespace TelegramBot.HelperModel
{
    public class ValidationResponse
    {
        public MessageModel? MessageModel { get; set; }
        public long ChatId { get; set; }
        public bool IsValidate { get; set; }
        public ValidationResponse(bool isValidate)
        {
            IsValidate = isValidate;
        }
        public ValidationResponse(MessageModel messageModel, long chatId, bool isValidate)
        {
            MessageModel = messageModel;
            ChatId = chatId;
            IsValidate = isValidate;
        }

    }
}
