public class BuildingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int TotalUnits { get; set; }
    public int? ConstructionYear { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public decimal? TotalArea { get; set; }
    public int TotalFloors { get; set; }
    public bool HasElevator { get; set; }
    public bool HasParking { get; set; }
    public bool HasGarden { get; set; }
    public bool HasPool { get; set; }
    public bool HasGym { get; set; }
    public string? ManagerName { get; set; }
    public string? ManagerPhone { get; set; }
    public string? ManagerEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int OccupiedUnits { get; set; }
    public int VacantUnits { get; set; }
    public decimal OccupancyRate { get; set; }
}