using System.Threading.Tasks;

namespace PROG7311_POE.Services
{
    // fixed rate converter
    public class FixedRateConverter : ICurrencyConverter
    {
        private readonly decimal _fixedRate;

        public FixedRateConverter(decimal fixedRate = 18.50m) // stores conversion rate
        {
            _fixedRate = fixedRate;
        }

        public Task<decimal> ConvertUsdToZar(decimal usdAmount)
        {
            return Task.FromResult(usdAmount * _fixedRate); // convers USD to ZAR (usses multiplication)
        }
    }
}