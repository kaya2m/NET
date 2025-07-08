using System.ComponentModel.DataAnnotations;

namespace NET.Application.DTOs.Tenant
{
    public class CreateTenantDto
    {
        [Required(ErrorMessage = "Tenant name is required")]
        [StringLength(255, ErrorMessage = "Tenant name cannot exceed 255 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subdomain is required")]
        [StringLength(100, ErrorMessage = "Subdomain cannot exceed 100 characters")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Subdomain can only contain lowercase letters, numbers, and hyphens")]
        public string Subdomain { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Contact email cannot exceed 255 characters")]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Contact phone cannot exceed 20 characters")]
        public string? ContactPhone { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "Tax number cannot exceed 50 characters")]
        public string? TaxNumber { get; set; }

        public DateTime? SubscriptionEndDate { get; set; }

        [Range(1, 100, ErrorMessage = "Max buildings must be between 1 and 100")]
        public int MaxBuildings { get; set; } = 1;

        [Range(1, 10000, ErrorMessage = "Max units must be between 1 and 10000")]
        public int MaxUnits { get; set; } = 100;
    }
}