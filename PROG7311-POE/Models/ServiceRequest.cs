using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using PROG7311_POE.Models.Transport;

namespace PROG7311_POE.Models
{
    // status of service request
    public enum RequestStatus
    {
        Pending,
        Approved,
        Completed,
        Cancelled
    }

    // types of transport - used for dropdown
    public enum TransportTypeEnum
    {
        Road,
        Air,
        Sea
    }

    public class ServiceRequest
    {
        public int Id { get; set; }
        // transport method
        public string TransportType { get; set; } = "Road";  // default - road

        [Required]          // contract ID
        public int ContractId { get; set; }
        [ValidateNever]     // skips validation - handled by contract ID
        public Contract Contract { get; set; } = null!;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]                 // cost cant be negative
        public decimal Cost { get; set; }            
        public decimal CostZar { get; set; }        // placeholder for API integration

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
