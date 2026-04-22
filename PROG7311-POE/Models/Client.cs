using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace PROG7311_POE.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required] // client is required
        public string Name { get; set; } = string.Empty;

        public string ContactDetails { get; set; } = string.Empty;  // contact info 

        public string Region { get; set; } = string.Empty;          // region info

        // one client can yhave many contracts 
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
