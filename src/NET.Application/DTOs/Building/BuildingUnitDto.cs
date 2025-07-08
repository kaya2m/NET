using NET.Domain.Enums;

namespace NET.Application.DTOs.Building
{
    public class BuildingUnitDto
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public UnitType UnitType { get; set; }
        public string UnitTypeName { get; set; } = string.Empty;
        public decimal? AreaSqm { get; set; }
        public bool IsOccupied { get; set; }
        public decimal? MonthlyDues { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public bool HasBalcony { get; set; }
        public string? Description { get; set; }
        public decimal? RentAmount { get; set; }
        public DateTime? LastOccupiedDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? CurrentResidentName { get; set; }
        public string? CurrentResidentPhone { get; set; }
        public DateTime? CurrentResidentStartDate { get; set; }
    }
}