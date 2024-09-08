using NLog;
using SelfProjectApi.Extentions.SQLExtentions;
using SelfProjectApi.Factories.CountryCodes;
using SelfProjectApi.Models.ApiResponses;
using SelfProjectApi.Models.Filter;
using SelfProjectApi.Models.Orders;
using SelfProjectApi.Models.Sales;
using SelfProjectApi.Models.Services;
using SelfProjectApi.Repository;
using static SelfProjectApi.Models.Services.CountryCodes;

namespace SelfProjectApi.Processing.OrderProcessing
{
    public class OrderProcessing : IOrderProcessing
    {
        private readonly IOrderSQLAccess _orderSQLAccess;
        private readonly NLog.ILogger _logger;
        private readonly ISQLExtentions _sqlExtentions;
        public OrderProcessing(IOrderSQLAccess orderSQLAccess, ISQLExtentions sqlExtentions)
        {
            _orderSQLAccess = orderSQLAccess;
            _sqlExtentions = sqlExtentions;
        }
        ///<inheritdoc/>
        public async Task<ApiRespone> AddNewOrders(List<BulkOrder> ordersToAdd, CountryCode countryCode)
        {
            try
            {
                //Set tax rule for country given
                ICountryCode countryProcessing = CountryCodeFactory.SetCountryTaxRule(countryCode);

                countryProcessing.ApplyCountryTaxCode(ordersToAdd);

                ApiRespone result = new();

                //attempt to post orders to db
                result = await _orderSQLAccess.AddNewOrdersAsyncEndpoint(ordersToAdd);

                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        ///<inheritdoc/>
        public async Task<ApiRespone> CancelOrder(string orderId, string userId)
        {
            try
            {
                return await _orderSQLAccess.CancelOrderAsyncEndpoint(orderId, userId);
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        ///<inheritdoc/>
        public async Task<List<Order>> GetOrdersAsync(Filter filter)
        {
            try
            {
                return await _orderSQLAccess.GetOrdersAsyncEndPoint(filter);
            }
            catch (Exception ex) 
            {
                _logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Parses the provided string into a corresponding CountryCode enum value.
        /// </summary>
        /// <param name="countryCode">The string representation of the country code to parse.</param>
        /// <returns>The parsed CountryCode enum value if successful.</returns>
        /// <exception cref="NotImplementedException">Thrown when the provided country code is unrecognized or null.</exception>
        public CountryCode ParseCountryCode(string countryCode)
        {
            if (Enum.TryParse<CountryCode>(countryCode, true, out var result))
            {
                return result;
            }
            else throw new NotImplementedException("Country code is unrecorgnised or null");
        }
    }
}
