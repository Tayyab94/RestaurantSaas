using RestaurantSaaS.Domain.Common;
using RestaurantSaaS.Domain.Enums;

namespace RestaurantSaaS.Domain.Entities;

/// <summary>RecipeStep: tenant table. Part of the Inventory module. See Database Schema.</summary>
public sealed class RecipeStep : TenantEntity
{
    public Guid RecipeId { get; set; }
    public int StepNumber { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public string? InstructionUrdu { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal? TemperatureC { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
