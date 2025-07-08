using System.ComponentModel.DataAnnotations;

namespace NET.Application.DTOs.Building
{
    public class UpdateBuildingDto
    {
        [Required(ErrorMessage = "Building name is required")]
        [StringLength(255, ErrorMessage = "Building name cannot exceed 255 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; } = string.Empty;

        [Range(1800, 2100, ErrorMessage = "Construction year must be between 1800 and 2100")]
        public int? ConstructionYear { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total area must be positive")]
        public decimal? TotalArea { get; set; }

        public bool HasElevator { get; set; }
        public bool HasParking { get; set; }
        public bool HasGarden { get; set; }
        public bool HasPool { get; set; }
        public bool HasGym { get; set; }

        [StringLength(100, ErrorMessage = "Manager name cannot exceed 100 characters")]
        public string? ManagerName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Manager phone cannot exceed 20 characters")]
        public string? ManagerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Manager email cannot exceed 255 characters")]
        public string? ManagerEmail { get; set; }

        public bool IsActive { get; set; } = true;
    }
}