namespace RestaurantSaaS.Application.Features.Settings;

public sealed record ModuleDefinition(string Key, string Name, string Description);

/// <summary>A business-type template: the code equals a BusinessType name. Modules maps a module key to its starting switch.</summary>
public sealed record BusinessTemplateDefinition(string Code, string Name, string Description, IReadOnlyDictionary<string, bool> Modules);

/// <summary>
/// The modules the Owner can switch on and off, and the starting switches of each business type.
/// The catalog is also written to the Feature, BusinessTypeTemplate and TemplateFeature tables at start-up
/// (see ModuleCatalogSeeder), so M34 plans can use the same keys.
/// A module with no saved switch uses the template default of the business type, and is on when there is none.
/// </summary>
public static class ModuleCatalog
{
    public const string Inventory = "module.inventory";
    public const string KitchenDisplay = "module.kitchen-display";
    public const string Delivery = "module.delivery";

    public static IReadOnlyList<ModuleDefinition> All { get; } =
    [
        new(Inventory, "Inventory", "Stock, recipes, suppliers and purchasing."),
        new(KitchenDisplay, "Kitchen display", "Orders shown on a kitchen screen instead of paper tickets."),
        new(Delivery, "Delivery", "Delivery orders, areas and riders."),
    ];

    public static IReadOnlyList<BusinessTemplateDefinition> Templates { get; } =
    [
        Template("Restaurant", "Restaurant", "Dine-in with a kitchen screen and delivery.", inventory: true, kitchenDisplay: true, delivery: true),
        Template("Cafe", "Cafe", "Counter and table service, no kitchen screen, no delivery.", inventory: true, kitchenDisplay: false, delivery: false),
        Template("Bakery", "Bakery", "Counter sales and delivery, no kitchen screen.", inventory: true, kitchenDisplay: false, delivery: true),
        Template("FastFood", "Fast food", "Quick service with a kitchen screen and delivery, no stock control.", inventory: false, kitchenDisplay: true, delivery: true),
        Template("Retail", "Retail shop", "Counter sales with stock, no kitchen screen, no delivery.", inventory: true, kitchenDisplay: false, delivery: false),
    ];

    private static BusinessTemplateDefinition Template(string code, string name, string description, bool inventory, bool kitchenDisplay, bool delivery) =>
        new(
            code,
            name,
            description,
            new Dictionary<string, bool>
            {
                [Inventory] = inventory,
                [KitchenDisplay] = kitchenDisplay,
                [Delivery] = delivery,
            });
}
