using NET.Domain.Entities.Common;
using System;
using System.Collections.Generic;

namespace NET.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? DatabaseName { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public int MaxBuildings { get; set; } = 1;
        public int MaxUnits { get; set; } = 100;

        // Navigation properties
        public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}