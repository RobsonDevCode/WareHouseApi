using SelfProjectApi.Factories.CountryCodes;
using SelfProjectApi.Models.Sales;

namespace SelfProjectApi.Processing.OrderProcessing.CountryCodeProcessing
{
    public class ProcessUSAOrders : ICountryCode
    {
        ///<inheritdoc/>
        public List<BulkOrder> ApplyCountryTaxCode(List<BulkOrder> orders)
        {
            Console.WriteLine($"Applying {this.GetType().Name} Logic");
            Product? maxPriceProduct = new Product();

            //completly random logic to simulate a loop and and some data manipluation, likley to happen within a tax method
            for (int i = 0; i < orders.Count(); i++)
            {
                if (orders[i].Product != null && orders[i].Product.Count() != 0)
                {
                    maxPriceProduct = orders[i].Product.OrderByDescending(p => p.Price).FirstOrDefault();

                    ApplyTax(maxPriceProduct);
                }

            }
            return orders;
        }

        private void ApplyTax(Product? products)
        {
            products.Price = products.Price / 0.123;
        }
    }
}
