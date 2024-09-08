using Microsoft.Extensions.Caching.Memory;

namespace SelfProjectApi.Models.Sales
{
    /// <summary>
    /// OrderMemoryCache: create a specficific instance of memeory caching for the order logic 
    /// </summary>
    public class OrderMemoryCache
    {
        public  MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 500,
        });
    }
}
