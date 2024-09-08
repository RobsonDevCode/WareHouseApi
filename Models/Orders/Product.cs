namespace SelfProjectApi.Models.Sales
{
    /// <summary>
    /// Class to represnt a product that been ordered.
    /// </summary>
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}