using SelfProjectApi.Models.Filter;
using SelfProjectApi.Models.Orders;
using SelfProjectApi.Models.Sales;

namespace SelfProjectApi.Processing.OrderProcessing.OrderCaching
{
    public interface IOrderApiCallCaching
    {
        /// <summary>
        /// OnGetOrCreateOrderCacheAsync: Gets any cached data related to the current filter, 
        /// if the method can't find any data related it will call the SQL endpoint and create a new cached entry for the request.
        /// </summary>
        /// <param name="filter">Filter model applied to the request</param>
        /// <returns cref="BulkOrder">A List of Order</returns>
        Task<List<Order>> OnGetOrCreateOrderCacheAsync(Filter filter);

    }
}
