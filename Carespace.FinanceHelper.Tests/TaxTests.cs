using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Carespace.FinanceHelper.Tests
{
    [TestClass]
    public sealed class TaxTests
    {
        [TestMethod]
        public void TestGetToken()
        {
            Configuration config = Helper.GetConfig();
            string token = DataManager.GetTaxToken(UserAgent, config.TaxSourceDeviceId, SourceType, AppVersion,
                config.TaxRefreshToken);
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        private const string UserAgent =
            "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Mobile Safari/537.36";
        private const string SourceType = "WEB";
        private const string AppVersion = "1.0.0";
    }
}
