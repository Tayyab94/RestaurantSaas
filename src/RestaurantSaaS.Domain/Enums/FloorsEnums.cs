namespace RestaurantSaaS.Domain.Enums;

public enum TableStatus
{
    Free,
    Occupied,
    Reserved,
    Billing,
}

public enum ReservationStatus
{
    Booked,
    Seated,
    Cancelled,
    NoShow,
    Completed,
}

public enum WaitingStatus
{
    Waiting,
    Seated,
    Left,
}

public enum QrSessionStatus
{
    Active,
    Submitted,
    Closed,
    Expired,
}
