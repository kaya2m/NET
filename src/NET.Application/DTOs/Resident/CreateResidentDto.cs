using System;
using System.ComponentModel.DataAnnotations;

namespace NET.Application.DTOs.Resident
{
    public class CreateResidentDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [StringLength(50, ErrorMessage = "Identity number cannot exceed 50 characters")]
        public string? IdentityNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(100, ErrorMessage = "Emergency contact cannot exceed 100 characters")]
        public string? EmergencyContact { get; set; }

        [Phone(ErrorMessage = "Invalid emergency contact phone format")]
        [StringLength(20, ErrorMessage = "Emergency contact phone cannot exceed 20 characters")]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(100, ErrorMessage = "Occupation cannot exceed 100 characters")]
        public string? Occupation { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        public bool IsOwner { get; set; } = false;

        public DateTime? MoveInDate { get; set; }
    }
}