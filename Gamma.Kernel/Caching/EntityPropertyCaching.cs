using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Gamma.Kernel.Attributes;

namespace Gamma.Kernel.Caching;

public class EntityPropertyCache<TEntity>
{
    private static readonly Lazy<EntityPropertyCache<TEntity>> _instance = new(() => new EntityPropertyCache<TEntity>());

    public static EntityPropertyCache<TEntity> Instance => _instance.Value;

    public PropertyInfo[] AllProperties { get; }
    public PropertyInfo[] InsertableProperties { get; }
    public PropertyInfo[] UpdatableProperties { get; }
    public PropertyInfo? IdentityProperty { get; }
    public PropertyInfo? RowVersionProperty { get; }

    public string TableName { get; }

    private EntityPropertyCache()
    {
        AllProperties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        InsertableProperties = [.. AllProperties
            .Where(p => p.CanWrite)
            .Where(p => p.GetCustomAttribute<IdentityAttribute>() is null)
            .Where(p => p.GetCustomAttribute<ComputedAttribute>() is null)];

        UpdatableProperties = [.. AllProperties
            .Where(p => p.CanWrite)
            .Where(p => p.Name != "Id")
            .Where(p => p.GetCustomAttribute<InsertOnlyAttribute>() is null)
            .Where(p => p.GetCustomAttribute<ComputedAttribute>() is null)
            .Where(p => p.GetCustomAttribute<IdentityAttribute>() is null)];

        IdentityProperty = AllProperties.FirstOrDefault(p => p.GetCustomAttribute<IdentityAttribute>() is not null);
        RowVersionProperty = AllProperties.FirstOrDefault(p => p.GetCustomAttribute<RowVersionAttribute>() is not null);

        // Table name with optional schema
        var tableAttr = typeof(TEntity).GetCustomAttribute<TableAttribute>();
        var schemaAttr = typeof(TEntity).GetCustomAttribute<SchemaAttribute>();

        TableName = schemaAttr is not null
            ? $"{schemaAttr.Name}.{tableAttr?.Name ?? typeof(TEntity).Name}"
            : tableAttr?.Name ?? typeof(TEntity).Name;
    }
}
