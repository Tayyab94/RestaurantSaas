namespace RestaurantSaaS.Domain.Enums;

public enum EmployeeSalaryType
{
    Monthly,
    Daily,
    Hourly,
}

public enum EmployeeStatus
{
    Active,
    OnLeave,
    Left,
}

public enum EmployeeEmploymentType
{
    Permanent,
    Contract,
    Daily,
}

public enum EmployeePayFrequency
{
    Monthly,
    Weekly,
    Daily,
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    Leave,
}

public enum AttendanceSource
{
    Manual,
    Pin,
    Biometric,
}

public enum LeaveRequestStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled,
}

public enum PayComponentComponentType
{
    Earning,
    Deduction,
}

public enum PayComponentCalcType
{
    Fixed,
    PercentOfBasic,
    PerUnit,
}

public enum PayrollRunStatus
{
    Draft,
    Reviewed,
    Approved,
    Paid,
    Cancelled,
}

public enum PayrollStatus
{
    Draft,
    Approved,
    Paid,
}

public enum PayrollItemItemType
{
    Basic,
    Allowance,
    Overtime,
    Commission,
    Tips,
    RiderEarning,
    Bonus,
    Fine,
    LoanInstallment,
    Statutory,
    Tax,
    Other,
}

public enum PayAdjustmentAdjustmentType
{
    Bonus,
    Fine,
    Allowance,
    Deduction,
}

public enum PayAdjustmentStatus
{
    Pending,
    Approved,
    Applied,
    Cancelled,
}

public enum TipPoolMethod
{
    Equal,
    ByHours,
    ByRole,
    Points,
}

public enum TipPoolStatus
{
    Draft,
    Approved,
    Paid,
}

public enum EmployeeLoanLoanType
{
    Advance,
    Loan,
}

public enum LoanStatus
{
    Requested,
    Approved,
    Active,
    Completed,
    Rejected,
    WrittenOff,
    Cancelled,
}

public enum InstallmentStatus
{
    Due,
    Deducted,
    Skipped,
    Rescheduled,
    PaidInCash,
}

public enum LoanTransactionTransactionType
{
    Issue,
    Deduction,
    CashRepayment,
    Adjustment,
    WriteOff,
}
