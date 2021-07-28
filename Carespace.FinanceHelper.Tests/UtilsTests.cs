using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Carespace.FinanceHelper.Tests
{
    [TestClass]
    public sealed class UtilsTests
    {
        [TestMethod]
        public void TestCalculateSharesNoPromo()
        {
            Configuration config = Helper.GetConfig();
            var t = new Transaction(100, 100, 1);
            TestCalculateShares(config, t, 50, 50);
        }

        [TestMethod]
        public void TestCalculateSharesAppearingShare()
        {
            Configuration config = Helper.GetConfig();
            var t1 = new Transaction(100, 100, 2);
            TestCalculateShares(config, t1, 100, 0);
            var t2 = new Transaction(100, 100, 2, "Promo2");
            TestCalculateShares(config, t2, 50, 50);
        }

        [TestMethod]
        public void TestCalculateSharesDisappearingShare()
        {
            Configuration config = Helper.GetConfig();
            var t1 = new Transaction(100, 100, 3);
            TestCalculateShares(config, t1, 50, 50);
            var t2 = new Transaction(100, 100, 3, "Promo3");
            TestCalculateShares(config, t2, 100, 0);
        }

        [TestMethod]
        public void TestCalculateSharesIncreasingShare()
        {
            Configuration config = Helper.GetConfig();
            var t1 = new Transaction(100, 100, 4);
            TestCalculateShares(config, t1, 50, 50);
            var t2 = new Transaction(100, 100, 4, "Promo4");
            TestCalculateShares(config, t2, 75, 25);
        }

        [TestMethod]
        public void TestCalculateSharesDecreasingShare()
        {
            Configuration config = Helper.GetConfig();
            var t1 = new Transaction(100, 100, 5);
            TestCalculateShares(config, t1, 50, 50);
            var t2 = new Transaction(100, 100, 5, "Promo5");
            TestCalculateShares(config, t2, 25, 75);
        }

        private static void TestCalculateShares(Configuration config, Transaction transaction, decimal shareAgent1,
            decimal shareAgent2)
        {
            Utils.CalculateShares(new[] { transaction }, 0, 0, config.PayMasterFeePercents, config.Shares);
            Assert.AreEqual(2, transaction.Shares.Count);
            Assert.IsTrue(transaction.Shares.ContainsKey(Agent1));
            Assert.IsTrue(transaction.Shares.ContainsKey(Agent2));
            Assert.AreEqual(shareAgent1, transaction.Shares[Agent1]);
            Assert.AreEqual(shareAgent2, transaction.Shares[Agent2]);
        }

        private const string Agent1 = "Agent1";
        private const string Agent2 = "Agent2";
    }
}
