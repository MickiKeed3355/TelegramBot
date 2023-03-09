namespace TelegramBot.IService.CommonModel
{
    public class BaseResponse
    {
        public string ErrorMessgae { get; set; }
        public bool Success { get => ErrorMessgae == null; }
        public BaseResponse()
        {

        }
        public BaseResponse(string errorMessgae)
        {
            ErrorMessgae = errorMessgae;
        }
    }
}
