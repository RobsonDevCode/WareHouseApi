using SelfProjectApi.Models.Orders;
using SelfProjectApi.Models.Sales;

namespace SelfProjectApi.Models.ApiResponses
{
    /// <summary>
    ///The OrderApiResponse class is used to encapsulate the response data from the Order API responses.
    /// It provides a structured way to return a list of orders along with pagination information, 
    /// such as the total number of pages. This approach enhances code clarity and maintainability 
    /// by grouping related data into a single response object, making it easier to manage and 
    /// extend the API response in the future.
    /// </summary>
    public class OrderApiResponse
    {
        public List<Order> Orders { get; set; }
        public int Pages { get; set; }
    }
}
