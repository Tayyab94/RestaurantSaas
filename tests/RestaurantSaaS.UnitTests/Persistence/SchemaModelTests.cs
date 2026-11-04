using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Domain.Common;

namespace RestaurantSaaS.UnitTests.Persistence;

/// <summary>
/// Checks the complete schema (all entity classes) as one EF Core model.
/// A configuration mistake in any of the tables makes the model fail to build, and these tests fail with it.
/// </summary>
public class SchemaModelTests
{
    private const int ExpectedTables = 236;

    private readonly TestDatabase _database = new();

    private static IReadOnlyList<Type> EntityClasses() =>
        typeof(BaseEntity).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(BaseEntity).IsAssignableFrom(t))
            .ToList();

    [Fact]
    public void The_whole_schema_builds_as_one_model()
    {
        using var context = _database.CreateContext();

        var modelTypes = context.Model.GetEntityTypes().Select(t => t.ClrType).ToHashSet();

        Assert.Equal(ExpectedTables, modelTypes.Count);
    }

    [Fact]
    public void Every_entity_class_is_a_table_of_the_model()
    {
        using var context = _database.CreateContext();
        var modelTypes = context.Model.GetEntityTypes().Select(t => t.ClrType).ToHashSet();

        Assert.Equal(ExpectedTables, EntityClasses().Count);
        Assert.All(EntityClasses(), type => Assert.Contains(type, modelTypes));
    }

    [Fact]
    public void Every_table_has_the_standard_columns()
    {
        using var context = _database.CreateContext();

        foreach (var entityType in context.Model.GetEntityTypes())
        {
            foreach (var column in new[] { "Id", "CreatedAt", "UpdatedAt", "IsDeleted" })
            {
                Assert.True(entityType.FindProperty(column) is not null, $"{entityType.ClrType.Name} has no {column}");
            }
        }
    }

    [Fact]
    public void Every_foreign_key_restricts_deletes()
    {
        using var context = _database.CreateContext();

        var offenders = context.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior != DeleteBehavior.Restrict)
            .Select(fk => $"{fk.DeclaringEntityType.ClrType.Name}.{string.Join(",", fk.Properties.Select(p => p.Name))}")
            .ToList();

        Assert.Empty(offenders);
    }

    [Fact]
    public void Tenant_and_branch_tables_have_the_tenant_and_branch_columns()
    {
        using var context = _database.CreateContext();

        foreach (var entityType in context.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                Assert.NotNull(entityType.FindProperty("TenantId"));
            }

            if (typeof(BranchEntity).IsAssignableFrom(entityType.ClrType))
            {
                Assert.NotNull(entityType.FindProperty("BranchId"));
            }
        }
    }
}
