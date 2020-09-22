using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarespaceFinanceHelper.Tests
{
    [TestClass]
    public sealed class DigisellerTests
    {
        [TestMethod]
        public void TestGetProductName()
        {
            string name = DataManager.GetDigisellerProductName(Url, UrlPrefix);
            Assert.AreEqual(ProductName, name);
        }

        private const string Url = "https://www.digiseller.market/asp2/pay_wm.asp?id_d=2957145";
        private const string UrlPrefix = "https://www.digiseller.market/asp2/pay_wm.asp?id_d=";
        private const string ProductName = "ННО: теория и практика";
    }
}
