using SelfProjectApi.Models.Sales;

namespace SelfProjectApi.Factories.CountryCodes
{
    public interface ICountryCode
    {
        /// <summary>
        /// ApplyCountryCode: for simplicity im only displaying a console message, i want to demonstrate i understand the factory software design 
        /// more than i want to demonstrate data manipulation.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="orders"></param>
        List<BulkOrder> ApplyCountryTaxCode (List<BulkOrder> orders);
    }
}
