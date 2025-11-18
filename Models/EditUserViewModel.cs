using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMonthlyClaim.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty; // Need ID to identify the user

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Hourly Rate")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, 10000)]
        public decimal HourlyRate { get; set; }

        // We won't allow changing the role in this simple edit view to avoid complexity
    }
}