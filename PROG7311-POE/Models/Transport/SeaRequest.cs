namespace PROG7311_POE.Models.Transport
{
    // defeines sea transport 
    public class SeaRequest : ITransportRequest
    {
        public string TransportType => "Sea";

        public decimal CalculateBaseCost(decimal inputCost)
        {
            // Sea freight: -10% (cheaper)
            return inputCost * 0.90m;
        }

        // description of sea transport
        public string GetDescription()
        {
            return "Sea transport – economical, slower";
        }
    }
}