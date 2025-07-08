using NET.Domain.Enums;

namespace NET.Application.DTOs.Maintenance
{
    public class UpdatePriorityDto
    {
        public Priority Priority { get; set; }
        public string? Reason { get; set; }
    }
}