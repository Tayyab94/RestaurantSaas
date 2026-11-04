using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence.Configurations;

public sealed class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.Property(x => x.ExpenseNumber).HasMaxLength(200);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.PaidTo).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Attachment).WithMany().HasForeignKey(x => x.AttachmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.ExpenseNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.Property(x => x.Code).HasMaxLength(100);
        builder.Property(x => x.FullName).HasMaxLength(200);
        builder.Property(x => x.CnicNumber).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(200);
        builder.Property(x => x.Designation).HasMaxLength(200);
        builder.Property(x => x.Department).HasMaxLength(200);
        builder.Property(x => x.BaseSalary).HasPrecision(18, 2);
        builder.Property(x => x.BankName).HasMaxLength(200);
        builder.Property(x => x.BankAccountNo).HasMaxLength(200);
        builder.Property(x => x.EobiNumber).HasMaxLength(200);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}

public sealed class WorkShiftConfiguration : IEntityTypeConfiguration<WorkShift>
{
    public void Configure(EntityTypeBuilder<WorkShift> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class EmployeeScheduleConfiguration : IEntityTypeConfiguration<EmployeeSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeSchedule> builder)
    {
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.WorkShift).WithMany().HasForeignKey(x => x.WorkShiftId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.EmployeeId, x.WorkDate }).IsUnique();
    }
}

public sealed class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.Property(x => x.Entitled).HasPrecision(18, 2);
        builder.Property(x => x.Taken).HasPrecision(18, 2);
        builder.Property(x => x.Balance).HasPrecision(18, 2);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.LeaveType).WithMany().HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique();
    }
}

public sealed class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.LeaveType).WithMany().HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
    }
}

public sealed class PayComponentConfiguration : IEntityTypeConfiguration<PayComponent>
{
    public void Configure(EntityTypeBuilder<PayComponent> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.NameUrdu).HasMaxLength(200);
        builder.Property(x => x.DefaultValue).HasPrecision(18, 2);
    }
}

public sealed class EmployeePayComponentConfiguration : IEntityTypeConfiguration<EmployeePayComponent>
{
    public void Configure(EntityTypeBuilder<EmployeePayComponent> builder)
    {
        builder.Property(x => x.Value).HasPrecision(18, 2);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PayComponent).WithMany().HasForeignKey(x => x.PayComponentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
{
    public void Configure(EntityTypeBuilder<PayrollRun> builder)
    {
        builder.Property(x => x.TotalEarnings).HasPrecision(18, 2);
        builder.Property(x => x.TotalDeductions).HasPrecision(18, 2);
        builder.Property(x => x.NetPay).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.BranchId, x.PeriodYear, x.PeriodMonth }).IsUnique();
    }
}

public sealed class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
{
    public void Configure(EntityTypeBuilder<Payroll> builder)
    {
        builder.Property(x => x.BasicSalary).HasPrecision(18, 2);
        builder.Property(x => x.Allowances).HasPrecision(18, 2);
        builder.Property(x => x.Overtime).HasPrecision(18, 2);
        builder.Property(x => x.Commission).HasPrecision(18, 2);
        builder.Property(x => x.Tips).HasPrecision(18, 2);
        builder.Property(x => x.Deductions).HasPrecision(18, 2);
        builder.Property(x => x.AdvanceRecovered).HasPrecision(18, 2);
        builder.Property(x => x.NetPay).HasPrecision(18, 2);
        builder.HasOne(x => x.PayrollRun).WithMany().HasForeignKey(x => x.PayrollRunId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.PayrollRunId, x.EmployeeId }).IsUnique();
    }
}

public sealed class PayrollItemConfiguration : IEntityTypeConfiguration<PayrollItem>
{
    public void Configure(EntityTypeBuilder<PayrollItem> builder)
    {
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.ReferenceType).HasMaxLength(50);
        builder.HasOne(x => x.Payroll).WithMany().HasForeignKey(x => x.PayrollId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PayComponent).WithMany().HasForeignKey(x => x.PayComponentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PayAdjustmentConfiguration : IEntityTypeConfiguration<PayAdjustment>
{
    public void Configure(EntityTypeBuilder<PayAdjustment> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PayrollItem).WithMany().HasForeignKey(x => x.PayrollItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class TipPoolConfiguration : IEntityTypeConfiguration<TipPool>
{
    public void Configure(EntityTypeBuilder<TipPool> builder)
    {
        builder.Property(x => x.TotalTips).HasPrecision(18, 2);
    }
}

public sealed class TipPoolShareConfiguration : IEntityTypeConfiguration<TipPoolShare>
{
    public void Configure(EntityTypeBuilder<TipPoolShare> builder)
    {
        builder.Property(x => x.ShareUnits).HasPrecision(18, 3);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.TipPool).WithMany().HasForeignKey(x => x.TipPoolId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PayrollItem).WithMany().HasForeignKey(x => x.PayrollItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class EmployeeLoanConfiguration : IEntityTypeConfiguration<EmployeeLoan>
{
    public void Configure(EntityTypeBuilder<EmployeeLoan> builder)
    {
        builder.Property(x => x.LoanNumber).HasMaxLength(200);
        builder.Property(x => x.PrincipalAmount).HasPrecision(18, 2);
        builder.Property(x => x.InstallmentAmount).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.OutstandingBalance).HasPrecision(18, 2);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CashMovement).WithMany().HasForeignKey(x => x.CashMovementId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.BranchId, x.LoanNumber }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ClientRequestId }).IsUnique();
    }
}

public sealed class LoanInstallmentConfiguration : IEntityTypeConfiguration<LoanInstallment>
{
    public void Configure(EntityTypeBuilder<LoanInstallment> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.DeductedAmount).HasPrecision(18, 2);
        builder.HasOne(x => x.Loan).WithMany().HasForeignKey(x => x.LoanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PayrollItem).WithMany().HasForeignKey(x => x.PayrollItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.DueYear, x.DueMonth, x.Status });
    }
}

public sealed class LoanTransactionConfiguration : IEntityTypeConfiguration<LoanTransaction>
{
    public void Configure(EntityTypeBuilder<LoanTransaction> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.BalanceAfter).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Loan).WithMany().HasForeignKey(x => x.LoanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PayrollItem).WithMany().HasForeignKey(x => x.PayrollItemId).OnDelete(DeleteBehavior.Restrict);
    }
}
