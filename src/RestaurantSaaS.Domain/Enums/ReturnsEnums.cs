namespace RestaurantSaaS.Domain.Enums;

public enum ReturnOrderReturnType
{
    Full,
    Partial,
}

public enum ReturnOrderRefundMethod
{
    Cash,
    Original,
    CustomerAccount,
}

public enum ReturnStatus
{
    Requested,
    Approved,
    Rejected,
    Completed,
}

public enum ReturnItemStockAction
{
    Restock,
    Wastage,
    None,
}
