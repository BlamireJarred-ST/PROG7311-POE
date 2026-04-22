namespace PROG7311_POE.Services
{
    // defines how to conver currency
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertUsdToZar(decimal usdAmount);
    }
}