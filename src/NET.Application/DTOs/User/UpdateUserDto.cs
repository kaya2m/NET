using NET.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace NET.Application.DTOs.User
{
    public class UpdateUserDto
    {
        public Guid? BuildingId { get; set; }

        public Guid? ResidentId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public UserRoleEnum Role { get; set; }

        public bool IsActive { get; set; } = true;
        public bool TwoFactorEnabled { get; set; } = false;
    }
}