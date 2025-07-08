using AutoMapper;
using NET.Application.DTOs.Building;
using NET.Application.DTOs.Financial;
using NET.Application.DTOs.Maintenance;
using NET.Application.DTOs.Resident;
using NET.Application.DTOs.Role;
using NET.Application.DTOs.Tenant;
using NET.Application.DTOs.User;
using NET.Domain.Entities;
using NET.Domain.Enums;
using System.Linq;
using UserRole = NET.Domain.Entities.UserRole;

namespace NET.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Building Mappings
            CreateMap<Building, BuildingDto>()
                .ForMember(dest => dest.OccupiedUnits, opt => opt.Ignore())
                .ForMember(dest => dest.VacantUnits, opt => opt.Ignore())
                .ForMember(dest => dest.OccupancyRate, opt => opt.Ignore());

            CreateMap<CreateBuildingDto, Building>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            CreateMap<UpdateBuildingDto, Building>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.TotalUnits, opt => opt.Ignore())
                .ForMember(dest => dest.TotalFloors, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // Building Unit Mappings
            CreateMap<BuildingUnit, BuildingUnitDto>()
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Building.Name))
                .ForMember(dest => dest.UnitTypeName, opt => opt.MapFrom(src => src.UnitType.ToString()))
                .ForMember(dest => dest.CurrentResidentName, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentResidentPhone, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentResidentStartDate, opt => opt.Ignore());

            CreateMap<CreateBuildingUnitDto, BuildingUnit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsOccupied, opt => opt.Ignore())
                .ForMember(dest => dest.LastOccupiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // Resident Mappings
            CreateMap<Resident, ResidentDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Units, opt => opt.MapFrom(src => src.ResidentUnits.Where(ru => ru.IsActive)));

            CreateMap<CreateResidentDto, Resident>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.MoveOutDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            //CreateMap<UpdateResidentDto, Resident>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            CreateMap<ResidentUnit, ResidentUnitInfo>()
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.BuildingUnitId))
                .ForMember(dest => dest.UnitNumber, opt => opt.MapFrom(src => src.BuildingUnit.UnitNumber))
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.BuildingUnit.Building.Name));

            // Payment Mappings
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.ResidentName, opt => opt.MapFrom(src => $"{src.Resident.FirstName} {src.Resident.LastName}"))
                .ForMember(dest => dest.UnitNumber, opt => opt.MapFrom(src => src.BuildingUnit != null ? src.BuildingUnit.UnitNumber : null))
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.BuildingUnit != null ? src.BuildingUnit.Building.Name : null))
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.InvoiceNumber : null))
                .ForMember(dest => dest.PaymentTypeName, opt => opt.MapFrom(src => src.PaymentType.ToString()))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PaymentStatus.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // Invoice Mappings
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.UnitNumber, opt => opt.MapFrom(src => src.BuildingUnit.UnitNumber))
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.BuildingUnit.Building.Name))
                .ForMember(dest => dest.ResidentName, opt => opt.MapFrom(src => src.Resident != null ? $"{src.Resident.FirstName} {src.Resident.LastName}" : null))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentTypeName, opt => opt.MapFrom(src => src.PaymentType.ToString()));

            CreateMap<CreateInvoiceDto, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Amount + (src.TaxAmount ?? 0)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => InvoiceStatus.Draft))
                .ForMember(dest => dest.PaidDate, opt => opt.Ignore())
                .ForMember(dest => dest.PaidAmount, opt => opt.Ignore())
                .ForMember(dest => dest.PdfPath, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            CreateMap<UpdateInvoiceDto, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.BuildingUnitId, opt => opt.Ignore())
                .ForMember(dest => dest.ResidentId, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceDate, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Amount + (src.TaxAmount ?? 0)))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PaidDate, opt => opt.Ignore())
                .ForMember(dest => dest.PaidAmount, opt => opt.Ignore())
                .ForMember(dest => dest.PdfPath, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // Expense Mappings
            CreateMap<Expense, ExpenseDto>()
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Building.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedByUser != null ? $"{src.ApprovedByUser.FirstName} {src.ApprovedByUser.LastName}" : null));

            CreateMap<CreateExpenseDto, Expense>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiptPath, opt => opt.Ignore())
                .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // Maintenance Request Mappings
            CreateMap<MaintenanceRequest, MaintenanceRequestDto>()
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Building.Name))
                .ForMember(dest => dest.UnitNumber, opt => opt.MapFrom(src => src.BuildingUnit != null ? src.BuildingUnit.UnitNumber : null))
                .ForMember(dest => dest.ResidentName, opt => opt.MapFrom(src => src.Resident != null ? $"{src.Resident.FirstName} {src.Resident.LastName}" : null))
                .ForMember(dest => dest.AssignedToUserName, opt => opt.MapFrom(src => src.AssignedToUser != null ? $"{src.AssignedToUser.FirstName} {src.AssignedToUser.LastName}" : null))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedByUser != null ? $"{src.ApprovedByUser.FirstName} {src.ApprovedByUser.LastName}" : null))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PriorityName, opt => opt.MapFrom(src => src.Priority.ToString()));

            //CreateMap<CreateMaintenanceRequestDto, MaintenanceRequest>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            //    .ForMember(dest => dest.AssignedToUserId, opt => opt.Ignore())
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MaintenanceStatus.Open))
            //    .ForMember(dest => dest.CompletedDate, opt => opt.Ignore())
            //    .ForMember(dest => dest.ActualCost, opt => opt.Ignore())
            //    .ForMember(dest => dest.WorkDescription, opt => opt.Ignore())
            //    .ForMember(dest => dest.PhotoUrls, opt => opt.Ignore())
            //    .ForMember(dest => dest.CompletionNotes, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
            //    .ForMember(dest => dest.ApprovedBy, opt => opt.Ignore())
            //    .ForMember(dest => dest.ApprovedDate, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            //CreateMap<UpdateMaintenanceRequestDto, MaintenanceRequest>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            //    .ForMember(dest => dest.BuildingId, opt => opt.Ignore())
            //    .ForMember(dest => dest.BuildingUnitId, opt => opt.Ignore())
            //    .ForMember(dest => dest.ResidentId, opt => opt.Ignore())
            //    .ForMember(dest => dest.AssignedToUserId, opt => opt.Ignore())
            //    .ForMember(dest => dest.Status, opt => opt.Ignore())
            //    .ForMember(dest => dest.CompletedDate, opt => opt.Ignore())
            //    .ForMember(dest => dest.PhotoUrls, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
            //    .ForMember(dest => dest.ApprovedBy, opt => opt.Ignore())
            //    .ForMember(dest => dest.ApprovedDate, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // User Mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Building != null ? src.Building.Name : null))
                .ForMember(dest => dest.ResidentName, opt => opt.MapFrom(src => src.Resident != null ? $"{src.Resident.FirstName} {src.Resident.LastName}" : null));

            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginDate, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginDate, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // Tenant Mappings
            //CreateMap<Tenant, TenantDto>()
            //    .ForMember(dest => dest.CurrentBuildingsCount, opt => opt.Ignore())
            //    .ForMember(dest => dest.CurrentUnitsCount, opt => opt.Ignore())
            //    .ForMember(dest => dest.CurrentUsersCount, opt => opt.Ignore());

            CreateMap<CreateTenantDto, Tenant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DatabaseName, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.SubscriptionStartDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.MaxBuildings, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.MaxUnits, opt => opt.MapFrom(src => 100))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            CreateMap<UpdateTenantDto, Tenant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Subdomain, opt => opt.Ignore())
                .ForMember(dest => dest.DatabaseName, opt => opt.Ignore())
                .ForMember(dest => dest.SubscriptionStartDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // Role Mappings
            CreateMap<Role, RoleDto>();
            CreateMap<CreateRoleDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsSystemRole, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // UserRole Mappings
            CreateMap<UserRole, UserRoleDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
        }
    }
}