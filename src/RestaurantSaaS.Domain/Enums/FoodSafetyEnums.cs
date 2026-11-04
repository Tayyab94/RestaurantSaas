namespace RestaurantSaaS.Domain.Enums;

public enum EquipmentEquipmentType
{
    Fridge,
    Freezer,
    Chiller,
    Oven,
    Fryer,
    HotHold,
    Other,
}

public enum ChecklistCategory
{
    Opening,
    Closing,
    Cleaning,
    Receiving,
    Other,
}

public enum FoodSafetyChecklistFrequency
{
    Daily,
    EveryShift,
    Weekly,
}

public enum ChecklistItemResultType
{
    YesNo,
    Number,
    Text,
    Photo,
}

public enum ChecklistRunStatus
{
    Open,
    Completed,
    Missed,
}

public enum TemperatureSource
{
    Manual,
    Sensor,
}

public enum FoodSafetyIncidentIncidentType
{
    Temperature,
    Contamination,
    Expiry,
    Pest,
    Complaint,
    Other,
}

public enum IncidentStatus
{
    Open,
    Closed,
}
