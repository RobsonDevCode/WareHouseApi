using SelfProjectApi.Models.SupplierInfo;

namespace SelfProjectApi.Models.Orders
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public int OrderNumber { get; set; }
        public string CountryCode { get; set; }
        public DateTime? DateCreated { get; set; }
        public Supplier SupplierDetails { get; set; }
    }
}
