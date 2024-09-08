using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NLog;
using SelfProjectApi.Configuration;
using SelfProjectApi.Models.ApiResponses;
using SelfProjectApi.Models.Filter;
using SelfProjectApi.Models.Sales;
using SelfProjectApi.Models.Services;
using SelfProjectApi.Processing.OrderProcessing;
using SelfProjectApi.Processing.OrderProcessing.OrderCaching;
using System.ComponentModel.DataAnnotations;
using static SelfProjectApi.Models.Services.CountryCodes;

namespace SelfProjectApi.Controllers.V1
{
    [Authorize] 
    [ApiController]
    [Route("v1/[controller]")] //we use versioning to ensure backwords compatibility 
    public class HomeController : ControllerBase
    {
        #region Constructor 
         private readonly IConfiguration _configuration;
         private readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
         private readonly IOrderApiCallCaching _orderApiCallCaching;
         private readonly OrderMemoryCache _orderCaching;
        private readonly IOrderProcessing _orderProcessing;
            
         public HomeController(IConfiguration configuration, OrderMemoryCache orderMemoryCache, IOrderApiCallCaching orderApiCallCaching, IOrderProcessing orderProcessing)
         {
            _configuration = configuration;
            _orderCaching = orderMemoryCache;
            _orderApiCallCaching = orderApiCallCaching;
            _orderProcessing = orderProcessing;
         }

        #endregion
        /// <summary>
        ///  GetOrdersAsync: Retrives orders from our sql db based on the filters applies asyncrounsly
        /// </summary>
        /// <param name="countryCode">Country code tied to the order made</param>
        /// <param name="searchTerm">Search on a order found in the database</param>
        /// <param name="pageSize">display size of the data requested</param>
        /// <param name="pageIndex">page index used to filter pages</param>
        /// <param name="isDescending">order of the data being displayed based on order id</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Exception"></exception>
        [HttpGet("GetOrders")]
        public async Task<OrderApiResponse> GetOrdersAsync(string? countryCode, string? searchTerm,[Required]int pageSize, [Required] int pageIndex, bool isDescending)
        {
            try
            {
                //map filter to model
                Filter filter = new Filter
                {
                    CountryCode = countryCode ?? string.Empty,
                    SupplierNameSearch = searchTerm,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    //set descending as default if its null 
                    IsDescending = isDescending
                };

                var result = await _orderApiCallCaching.OnGetOrCreateOrderCacheAsync(filter);

                OrderApiResponse orderApiResponse = new()
                {
                    Orders = result.Skip((filter.PageIndex - 1) * filter.PageSize).Take(pageSize).ToList(),
                    //cast as an int as pages so it rounds up to a whole number 
                    //cast doublle so if the result is odd we round up
                    Pages = (int)Math.Ceiling((double)result.Count() / filter.PageSize)
                };

                return orderApiResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// AddNewOrders: Stores 
        /// </summary>
        /// <param name="ordersToAdd">List of orders to add</param>
        /// <param name="countryCode">Country the orders are being sent</param>
        /// <returns cref="ApiRespone">Api response based on the success or failure of the request </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        [HttpPost("AddNewOrders")]
        public async Task<ApiRespone> AddNewOrders([FromBody]List<BulkOrder> ordersToAdd, [FromBody]string countryCode)
        {

            ApiRespone apiResponse = new ApiRespone();

            try
            {
                if (string.IsNullOrWhiteSpace(countryCode)) throw new ArgumentNullException("Counry code cannot be null when creating a store order");

                if (ordersToAdd.Count() == 0) throw new ArgumentNullException("Orders accepted are null");


                //pair the order to the country code 
                CountryCode countryCodeAsEnum = _orderProcessing.ParseCountryCode(countryCode);

                apiResponse = await _orderProcessing.AddNewOrders(ordersToAdd, countryCodeAsEnum);
                
                return apiResponse;

            }

            catch(Exception ex)
            {
                string errorMessage = ex.Message;

                if (!string.IsNullOrEmpty(apiResponse.Message))
                {
                    errorMessage = $"{ex.Message} Status code returned: {apiResponse.StatusCode}";
                }

                _logger.Error(errorMessage);

               throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// CancelOrder: Cancel Order request based on the order id.
        /// </summary>
        /// <param name="orderId">id of the order requested to be deleted</param>
        /// <param name="userId">user deleting the order</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Api response based on the success or failure of the request</exception>
        //Set so only admins can cancel orders
        [Authorize(policy: Settings.AdminUserPolicyName)]
        [HttpDelete("CancelOrders/{orderId}")]
        public async Task<ApiRespone> CancelOrder([FromRoute]string orderId, [FromRoute]string userId)
        {
            ApiRespone respone = new ApiRespone();
            try
            {
                if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentNullException("Error accepting request: orderId is Null");

                if(string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("Error accepting request: userId is Null");

                respone = await _orderProcessing.CancelOrder(orderId, userId);

                return respone;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
