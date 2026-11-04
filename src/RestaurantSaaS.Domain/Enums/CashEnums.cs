namespace RestaurantSaaS.Domain.Enums;

public enum ShiftStatus
{
    Open,
    Closed,
}

public enum CashMovementMovementType
{
    CashIn,
    CashOut,
    Float,
    Drop,
    EmployeeLoan,
}

public enum CashDrawerReason
{
    Sale,
    NoSale,
    CashMovement,
}

public enum DayClosingStatus
{
    Closed,
    Reopened,
}
