using SelfProjectApi.Models.ApiResponses;
using SelfProjectApi.Models.Filter;
using SelfProjectApi.Models.Orders;
using SelfProjectApi.Models.Sales;

namespace SelfProjectApi.Repository
{
    public interface IOrderSQLAccess
    {

        /// <summary>
        /// GetOrdersAsyncEndPoint: SQL end point for Orders based on the filter accepted
        /// </summary>
        /// <param name="filter"></param>
        /// <returns cref="BulkOrder">List of Orders</returns>
        Task<List<Order>> GetOrdersAsyncEndPoint(Filter filter);

        /// <summary>
        /// AddNewOrdersAsyncEndpoint: SQL end point to add store orders to sql
        /// </summary>
        /// <param name="ordersToAdd"></param>
        /// <returns></returns>
        Task<ApiRespone> AddNewOrdersAsyncEndpoint(List<BulkOrder> ordersToAdd);
        /// <summary>
        /// CancelOrderAsyncEndpoint: SQL end point to cancel an order request on the SQL db
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ApiRespone> CancelOrderAsyncEndpoint(string orderId, string userId);
    }
}
