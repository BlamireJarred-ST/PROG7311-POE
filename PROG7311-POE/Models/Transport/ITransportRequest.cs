namespace PROG7311_POE.Models.Transport
{
    //defines transport cost
    public interface ITransportRequest
    {
        string TransportType { get; } // gets type of transport
        decimal CalculateBaseCost(decimal inputCost); // adjust cost based on transport type
        string GetDescription(); // description of transport
    }
}