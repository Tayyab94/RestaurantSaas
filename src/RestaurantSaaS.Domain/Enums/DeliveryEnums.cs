namespace RestaurantSaaS.Domain.Enums;

public enum RiderCommissionType
{
    None,
    PerDelivery,
    Percent,
}

public enum RiderPayModel
{
    Salary,
    PerDelivery,
    SalaryPlusPerDelivery,
}

public enum RiderStatus
{
    Available,
    Busy,
    Off,
}

public enum DeliveryDeliveryType
{
    Rider,
    CustomerPickup,
    ThirdParty,
}

public enum DeliveryStatus
{
    Pending,
    Assigned,
    OutForDelivery,
    Delivered,
    Failed,
    Returned,
}

public enum RiderSettlementShortageHandling
{
    Deduct,
    Waive,
    Pending,
}

public enum RiderEarningEarningType
{
    Commission,
    FuelAllowance,
    Bonus,
    Penalty,
    ShortageDeduction,
}

public enum RiderEarningStatus
{
    Open,
    Included,
    Paid,
}
