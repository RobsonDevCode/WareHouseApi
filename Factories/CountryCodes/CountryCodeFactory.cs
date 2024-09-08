using SelfProjectApi.Models.Services;
using SelfProjectApi.Processing.OrderProcessing.CountryCodeProcessing;
using static SelfProjectApi.Models.Services.CountryCodes;

namespace SelfProjectApi.Factories.CountryCodes
{
    public class CountryCodeFactory
    {
        /// <summary>
        /// SetCountryTaxRule: set order tax rules based on the country code parsed.
        /// </summary>
        /// <param name="countryCodes"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static ICountryCode SetCountryTaxRule(CountryCode countryCodes)
        {
            switch(countryCodes)
            {
              
                case CountryCode.USA:
                    return new ProcessUSAOrders();

                case CountryCode.UK:
                    return new ProcessUKOrders();

                case CountryCode.AUS:
                    return new ProcessAustralianOrders();

                case CountryCode.ESP:
                    return new ProcessSpanishOrders();

                case CountryCode.JPN: 
                    return new ProcessJapaneseOrders();

                default :
                    throw new NotImplementedException($"Country code: {countryCodes} is not recognised");
            }
        }
    }
}
