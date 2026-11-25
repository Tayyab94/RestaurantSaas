using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Application.Features.Settings;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence;

/// <summary>
/// Writes the module catalog (M05) to the platform tables: Feature, BusinessTypeTemplate and TemplateFeature.
/// It only adds what is missing and refreshes names, so running it at every start is safe.
/// </summary>
public static class ModuleCatalogSeeder
{
    public static async Task EnsureAsync(AppDbContext db, CancellationToken ct = default)
    {
        var features = await db.Set<Feature>().ToDictionaryAsync(f => f.Key, ct);

        foreach (var module in ModuleCatalog.All)
        {
            if (features.TryGetValue(module.Key, out var feature))
            {
                feature.Name = module.Name;
                feature.Description = module.Description;
            }
            else
            {
                feature = new Feature { Key = module.Key, Name = module.Name, Description = module.Description };
                db.Set<Feature>().Add(feature);
                features[module.Key] = feature;
            }
        }

        var templates = await db.Set<BusinessTypeTemplate>().ToDictionaryAsync(t => t.Code, ct);

        foreach (var definition in ModuleCatalog.Templates)
        {
            if (templates.TryGetValue(definition.Code, out var template))
            {
                template.Name = definition.Name;
            }
            else
            {
                template = new BusinessTypeTemplate { Code = definition.Code, Name = definition.Name };
                db.Set<BusinessTypeTemplate>().Add(template);
                templates[definition.Code] = template;
            }
        }

        // Ids are created by the entities, so the new rows can be linked before they are saved.
        var existing = await db.Set<TemplateFeature>()
            .Select(x => new { x.TemplateId, x.FeatureId })
            .ToListAsync(ct);

        foreach (var definition in ModuleCatalog.Templates)
        {
            var template = templates[definition.Code];

            foreach (var (key, enabled) in definition.Modules)
            {
                var feature = features[key];

                if (existing.Any(x => x.TemplateId == template.Id && x.FeatureId == feature.Id))
                {
                    continue;
                }

                db.Set<TemplateFeature>().Add(new TemplateFeature
                {
                    TemplateId = template.Id,
                    FeatureId = feature.Id,
                    IsEnabledByDefault = enabled,
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
