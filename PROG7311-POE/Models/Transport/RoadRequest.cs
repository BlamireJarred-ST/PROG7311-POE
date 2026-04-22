namespace PROG7311_POE.Models.Transport
{
    // defines road transport
    public class RoadRequest : ITransportRequest
    {
        public string TransportType => "Road";

        public decimal CalculateBaseCost(decimal inputCost)
        {
            // Road freight: standard rate
            return inputCost;
        }

        // description of road transport
        public string GetDescription()
        {
            return "Road transport – flexible regional delivery";
        }
    }
}