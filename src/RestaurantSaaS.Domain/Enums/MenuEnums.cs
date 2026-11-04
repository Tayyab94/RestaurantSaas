namespace RestaurantSaaS.Domain.Enums;

public enum MenuItemItemType
{
    Food,
    Drink,
    Retail,
    Service,
}

public enum MenuItemStockMode
{
    None,
    Recipe,
    Direct,
}

public enum BranchMenuItemOutSource
{
    Kitchen,
    Inventory,
    Admin,
}

public enum AllergenPresence
{
    Contains,
    MayContain,
}

public enum AllergenSource
{
    Manual,
    FromRecipe,
}
