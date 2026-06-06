using Gamma.Kernel.Caching;
using Gamma.Kernel.Data;
using Gamma.Kernel.Entities;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Gamma.Kernel.Dapper;

public abstract partial class BaseSql<TEntity>
    where TEntity : BaseEntity
{
    public static readonly string TableName;
    public static readonly string TableAlias;
    protected static readonly Dictionary<string, string> AutoColumnMapping;

    static BaseSql()
    {
        var type = typeof(TEntity);
        var cache = EntityPropertyCache<TEntity>.Instance;

        TableName = SqlDialectResolver.EscapeDefaultIdentifier(cache.TableName);
        TableAlias = BuildAlias(type);

        AutoColumnMapping = cache.AllProperties
            .ToDictionary(
                p => p.Name,
                p => $"{TableAlias}.[{p.Name}]",
                StringComparer.OrdinalIgnoreCase
            );
    }

    protected static string Column(
        Expression<Func<TEntity, object?>> property,
        string? alias = null)
    {
        var column = ExpressionHelper.GetPropertyName(property);

        alias = string.IsNullOrWhiteSpace(alias)
            ? TableAlias
            : SqlDialectResolver.EscapeDefaultIdentifier(alias);

        return $"{alias}.[{column}]";
    }

    protected static SqlBuilder BaseSelect()
    {
        var sql = new SqlBuilder();

        foreach (var column in AutoColumnMapping.Values)
        {
            sql.Select(column);
        }

        return sql.From($"{TableName} {TableAlias}");
    }

    private static string BuildAlias(Type type)
    {
        var name = type.Name;

        // 1. PascalCase initials (UserRole -> ur)
        var initials = string.Concat(
            MyRegex().Matches(name)
                 .Select(m => m.Value.ToLower())
        );

        // 2. Ensure length 2–3
        var alias = initials.Length switch
        {
            >= 2 => initials[..Math.Min(3, initials.Length)],
            _ => name[..Math.Min(3, name.Length)].ToLower()
        };

        // 3. Deterministic conflict prevention (type hash suffix)
        var hash = Math.Abs(type.FullName!.GetHashCode() % 100);
        return $"[{alias}{hash}]";
    }

    [GeneratedRegex("[A-Z]")]
    private static partial Regex MyRegex();
}