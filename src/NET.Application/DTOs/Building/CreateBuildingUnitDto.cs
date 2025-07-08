using NET.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace NET.Application.DTOs.Building
{
    public class CreateBuildingUnitDto
    {
        [Required(ErrorMessage = "Building ID is required")]
        public Guid BuildingId { get; set; }

        [Required(ErrorMessage = "Unit number is required")]
        [StringLength(10, ErrorMessage = "Unit number cannot exceed 10 characters")]
        public string UnitNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Floor number is required")]
        [Range(-5, 200, ErrorMessage = "Floor number must be between -5 and 200")]
        public int FloorNumber { get; set; }

        [Required(ErrorMessage = "Unit type is required")]
        public UnitType UnitType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Area must be positive")]
        public decimal? AreaSqm { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Monthly dues must be positive")]
        public decimal? MonthlyDues { get; set; }

        [Range(0, 20, ErrorMessage = "Bedrooms must be between 0 and 20")]
        public int? Bedrooms { get; set; }

        [Range(0, 10, ErrorMessage = "Bathrooms must be between 0 and 10")]
        public int? Bathrooms { get; set; }

        public bool HasBalcony { get; set; } = false;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Rent amount must be positive")]
        public decimal? RentAmount { get; set; }
    }
}