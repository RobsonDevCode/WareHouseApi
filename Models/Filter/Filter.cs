using SelfProjectApi.Models.Services;
using static SelfProjectApi.Models.Services.CountryCodes;

namespace SelfProjectApi.Models.Filter
{
    public class Filter
    {
        public string CountryCode { get; set; }
        public string? SupplierNameSearch { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool IsDescending { get; set; }

    }
}
