using System;

namespace PROG7311_POE.Models.Transport
{
    // used to find transport based on user input
    public static class TransportFactory
    {
        public static ITransportRequest CreateRequest(string transportType)
        {
            return transportType.ToLower() switch
            {
                // matches user input to class - not case sensitive
                "air" => new AirRequest(),
                "sea" => new SeaRequest(),
                "road" => new RoadRequest(),
                _ => throw new ArgumentException($"Unknown transport type: {transportType}")
            };
        }
    }
}