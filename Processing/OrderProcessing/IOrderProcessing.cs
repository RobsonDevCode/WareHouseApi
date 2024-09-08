using SelfProjectApi.Models.ApiResponses;
using SelfProjectApi.Models.Filter;
using SelfProjectApi.Models.Orders;
using SelfProjectApi.Models.Sales;
using SelfProjectApi.Models.Services;
using static SelfProjectApi.Models.Services.CountryCodes;

namespace SelfProjectApi.Processing.OrderProcessing
{
    public interface IOrderProcessing
    {
        /// <summary>
        /// GetOrdersAsync: Applies Filter Paramter to request, then retrive a List of Orders from the Order SQL Endpoint
        /// </summary>
        /// <param name="filter"></param>
        /// <returns cref="BulkOrder">A List of Orders</returns>
        Task<List<Order>> GetOrdersAsync(Filter filter);

        /// <summary>
        /// AddNewOrders: Attempts to Add a new batch of orders to the Orders Database
        /// </summary>
        /// <param name="ordersToAdd"></param>
        /// <returns cref="ApiRespone">A status code and a message based on if the request was succesful or not</returns>
        Task<ApiRespone> AddNewOrders(List<BulkOrder> ordersToAdd, CountryCode countryCode);

        /// <summary>
        /// CancelOrder: Cancel order placed based on the orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ApiRespone> CancelOrder(string orderId, string userId);
        /// <summary>
        /// Method to map country code to the enum CountryCode
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns cref="CountryCode"></returns>
        CountryCode ParseCountryCode(string countryCode);

    }
}
