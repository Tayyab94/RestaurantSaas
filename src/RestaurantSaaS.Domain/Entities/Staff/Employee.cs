using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>Employee: tenant table. Part of the Staff module. See Database Schema.</summary>
public sealed class Employee : TenantEntity
{
    public Guid? UserId { get; set; }
    public Guid BranchId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? CnicNumber { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateOnly JoinDate { get; set; }
    public DateOnly? LeaveDate { get; set; }
    public EmployeeSalaryType SalaryType { get; set; }
    public decimal BaseSalary { get; set; }
    public EmployeeStatus Status { get; set; }
    public EmployeeEmploymentType EmploymentType { get; set; }
    public EmployeePayFrequency PayFrequency { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNo { get; set; }
    public string? EobiNumber { get; set; }
    public bool IsRider { get; set; }

    public User? User { get; set; }
    public Branch Branch { get; set; } = null!;
}
