using PROG7311_POE.Models;

namespace PROG7311_POE.ViewModels
{
    // ViewModel for creating a contract - handles form data and file upload
    public class ContractCreateViewModel
    {
        public int ClientId { get; set; }

        // default start date set to today
        public DateTime StartDate { get; set; } = DateTime.Today;

        // default end date set to one year from today
        public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);

        // new contracts draft by default
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        // helps determine pricing
        public string ServiceLevel { get; set; } = string.Empty;

        // the uploaded PDF file 
        public IFormFile? SignedAgreement { get; set; }
    }
}