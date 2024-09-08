using SelfProjectApi.Models.Orders;
using SelfProjectApi.Models.SupplierInfo;

namespace SelfProjectApi.Models.Sales
{
    /// <summary>
    /// Class to represent and store Order variables from our database
    /// </summary>
    public class BulkOrder
    {
        public Guid OrderId { get; set; }
        public int OrderNumber { get; set; }
        public string CountryCode { get; set; }
        public DateTime? DateCreated { get; set; }
        public Supplier SupplierDetails { get; set; }
        public List<Product> Product { get; set; }

    }
}
