namespace TelegramBot.Service.Test
{
    [TestClass]
    public class DishesSaveServiceTest
    {

        private IDishesSaveService _dishesSaveService;

        /// <summary>
        /// ������������� ������ ��� ������� ������
        /// </summary>
        /// <returns></returns>
        [ClassInitialize]
        public async Task Init()
        {
            _dishesSaveService = DependencyResolver.GetService<IDishesSaveService>();
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        /// <param name="request"></param>
        /// <param name="expectedResult"></param>
        [DataTestMethod]
        [SaveDish]
        public void SaveDish(SaveDishRequest request, bool expectedResult)
        {
            var result = _dishesSaveService.SaveDish(request);
            Assert.AreEqual(expectedResult, result);
        }
    }
}