namespace PROG7311_POE.Models.Transport
{
    // defines air transport
    public class AirRequest : ITransportRequest
    {
        public string TransportType => "Air";

        public decimal CalculateBaseCost(decimal inputCost)
        {
            // Air freight premium: +30%
            return inputCost * 1.30m;
        }

        // defenition of air transport
        public string GetDescription()
        {
            return "Air transport – expedited shipping";
        }
    }
}