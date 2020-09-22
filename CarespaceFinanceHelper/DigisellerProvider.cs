using System.Collections.Generic;
using CarespaceFinanceHelper.Dto.Digiseller;

namespace CarespaceFinanceHelper
{
    internal static class DigisellerProvider
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

        private const string ApiProvider = "https://api.digiseller.ru/";
        private const string ProductsInfoMethod = "api/products/info";
    }
}
