namespace SelfProjectApi.Models.SupplierInfo
{
    /// <summary>
    /// Class to represent Supplier data 
    /// </summary>
    public class Supplier
    {
        public string Id { get; set; } = string.Empty;
        public string FullName  { get; set; } = string.Empty;
        public string CardNumber { get; set; }

    }
}