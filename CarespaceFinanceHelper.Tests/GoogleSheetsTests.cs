using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarespaceFinanceHelper.Tests
{
    [TestClass]
    public sealed class GoogleSheetsTests
    {
        [TestMethod]
        public void TestGoogleSheetsProvider()
        {
            Configuration config = GetConfig();
            using (var provider = new GoogleSheetsProvider(config.GoogleCredentialsJson, config.GoogleSheetId))
            {
                Assert.IsNotNull(provider);
            }
        }

        [TestMethod]
        public void TestGetValues()
        {
            Configuration config = GetConfig();
            using (var provider = new GoogleSheetsProvider(config.GoogleCredentialsJson, config.GoogleSheetId))
            {
                IList<Row> rows = DataManager.GetValues<Row>(provider, GetRange);
                Assert.IsNotNull(rows);
                Assert.AreEqual(2, rows.Count);
                CheckRow(Comment1, Date1, Amount1, rows[0]);
                CheckRow(Comment2, Date2, Amount2, rows[1]);
            }
        }

        [TestMethod]
        public void TestAppendValues()
        {
            Configuration config = GetConfig();
            using (var provider = new GoogleSheetsProvider(config.GoogleCredentialsJson, config.GoogleSheetId))
            {
                DataManager.AppendValues(provider, GetRange, new[] { Row });
                IList<Row> rows = DataManager.GetValues<Row>(provider, UpdateRange);
                Assert.IsNotNull(rows);
                Assert.AreEqual(1, rows.Count);
                CheckRow(Row, rows[0]);

                DataManager.AppendValues(provider, UpdateRange, new[] { EmptyRow }, true);
                rows = DataManager.GetValues<Row>(provider, UpdateRange);
                Assert.IsNull(rows);
            }
        }

        private static void CheckRow(Row expected, Row actual)
        {
            Assert.IsNotNull(expected);
            CheckRow(expected.Comment, expected.Date, expected.Amount, actual);
        }

        private static void CheckRow(string comment, DateTime? date, decimal? amount, Row row)
        {
            Assert.IsNotNull(row);
            Assert.AreEqual(comment, row.Comment);
            Assert.AreEqual(date, row.Date);
            Assert.AreEqual(amount, row.Amount);
        }

        private static Configuration GetConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json") // Create appsettings.json for private settings
                .Build()
                .Get<Configuration>();
        }

        private const string GetRange = "Test!A2:C4";
        private const string UpdateRange = "Test!A4:C4";

        private const string Comment1 = "Test1";
        private static readonly DateTime Date1 = new DateTime(2020, 8, 18);
        private const decimal Amount1 = -240;

        private const string Comment2 = "Test2";
        private static readonly DateTime Date2 = new DateTime(2020, 8, 19);
        private const decimal Amount2 = 319.2m;

        private static readonly Row Row = new Row
        {
            Comment = "Test",
            Date = DateTime.Today,
            Amount = 3.14m
        };

        private static readonly Row EmptyRow = new Row
        {
            Comment = "",
            Date = null,
            Amount = null
        };
    }
}
