public class ResidentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public string? IdentityNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Occupation { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsOwner { get; set; }
    public DateTime? MoveInDate { get; set; }
    public DateTime? MoveOutDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ResidentUnitInfo> Units { get; set; } = new();
}
public class ResidentUnitInfo
{
    public Guid UnitId { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
    public bool IsPrimaryResident { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}