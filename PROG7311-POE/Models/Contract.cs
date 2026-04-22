using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG7311_POE.Models
{
    // status of contract
    public enum ContractStatus
    {
        Draft,
        Active,
        Expired,
        OnHold
    }

    public class Contract
    {
        public int Id { get; set; }

        [Required]      // contract must have a client
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!; // client cant be null, it is null now but it wont be when it is called therefore it is set to not null

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        // service level agreement - used to set cost of transport
        public string ServiceLevel { get; set; } = string.Empty;

        // Path to the uploaded PDF file 
        public string SignedAgreementPath { get; set; } = string.Empty;

        // a contract can have many service requests
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
