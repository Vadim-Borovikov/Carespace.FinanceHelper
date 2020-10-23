using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Carespace.FinanceHelper.Dto.Digiseller;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Carespace.FinanceHelper.Providers
{
    internal static class Digiseller
    {
        public static ProductResult GetProductsInfo(int productId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "format", "json" },
                { "transp", "cors" },
                { "product_id", productId }
            };

            return RestHelper.CallGetMethod<ProductResult>(ApiProvider, ProductsInfoMethod, parameters);
        }

        public static SellsResult GetSells(int sellerId, List<int> productIds, string start, string end,
            int page, string sellerSecret)
        {
            string sign = Hash($"{sellerId}{string.Join("", productIds)}{start}{end}{Returned}{page}{RowsPerPage}{sellerSecret}");
            var dto = new SellsRequest
            {
                SellerId = sellerId,
                ProductIds = productIds,
                DateStart = start,
                DateFinish = end,
                Returned = Returned,
                Rows = RowsPerPage,
                Page = page,
                Sign = sign
            };

            return RestHelper.CallPostMethod<SellsResult>(ApiProvider, GetSellsMethod, dto, Settings);
        }

        public static TokenResult GetToken(string login, string password, string sellerSecret)
        {
            long timestamp = DateTime.Now.ToFileTime();
            string sign = Hash($"{password}{sellerSecret}{timestamp}");
            var dto = new TokenRequest
            {
                Login = login,
                Timestamp = timestamp,
                Sign = sign
            };

            return RestHelper.CallPostMethod<TokenResult>(ApiProvider, GetTokenMethod, dto, Settings);
        }

        public static PurchaseResult GetPurchase(int invoiceId, string token)
        {
            var parameters = new Dictionary<string, object>
            {
                { "token", token }
            };

            return RestHelper.CallGetMethod<PurchaseResult>(ApiProvider, $"{GetPurchaseMethod}{invoiceId}", parameters);
        }

        private static string Hash(string input)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(input);
            var sb = new StringBuilder();
            using (var sha256 = new SHA256Managed())
            {
                byte[] hash = sha256.ComputeHash(utf8);
                foreach (byte b in hash)
                {
                    sb.Append($"{b:x2}");
                }
            }
            return sb.ToString();
        }

        private static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = ContractResolver
        };

        private const string ApiProvider = "https://api.digiseller.ru/";
        private const string ProductsInfoMethod = "api/products/info";
        private const string GetSellsMethod = "api/seller-sells";
        private const string GetTokenMethod = "api/apilogin";
        private const string GetPurchaseMethod = "api/purchase/info/";

        private const int RowsPerPage = 2000;
        private const int Returned = 1;
    }
}
