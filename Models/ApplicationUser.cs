using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMonthlyClaim.Models
{
    public class ApplicationUser : IdentityUser
    {
        // NEW PROPERTIES REQUIRED BY PART 3
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal HourlyRate { get; set; }
    }
}