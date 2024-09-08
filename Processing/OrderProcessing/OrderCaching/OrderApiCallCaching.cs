using Microsoft.Extensions.Caching.Memory;
using NLog;
using SelfProjectApi.Models.Filter;
using SelfProjectApi.Models.Orders;
using SelfProjectApi.Models.Sales;

namespace SelfProjectApi.Processing.OrderProcessing.OrderCaching
{
    public class OrderApiCallCaching : IOrderApiCallCaching
    {
        #region Constructor

        private string _cacheKey;
        private OrderMemoryCache _orderCache;
        private IOrderProcessing _orderProccessing;
        private NLog.ILogger _logger;
        public OrderApiCallCaching(OrderMemoryCache orderMemoryCache, IOrderProcessing orderProccessing)
        {
            _orderCache = orderMemoryCache;
            _orderProccessing = orderProccessing;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        ///<inheritdoc/>
        public async Task<List<Order>> OnGetOrCreateOrderCacheAsync(Filter filter)
        {
            try
            {
                //cache settings
                _cacheKey = "order-cache-key";

                List<Order>? orders = new List<Order>();

                //Search to see if a value has been cached
                orders = _orderCache.Cache.Get<List<Order>>(_cacheKey);

                //if not pull from SQL and create and cache the data
                if (orders == null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                                            .SetSize(250)
                                            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                    orders = await _orderProccessing.GetOrdersAsync(filter);

                    _orderCache.Cache.Set(_cacheKey, orders, cacheEntryOptions);
                }

                else
                {
                    //add filter if to cached data
                    if (filter.IsDescending)
                    {
                        orders = orders.OrderByDescending(x => x.OrderNumber).ToList();
                    }
                    else
                    {
                        orders = orders.OrderBy(x => x.OrderNumber).ToList();
                    }
                
                }

                return orders;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
