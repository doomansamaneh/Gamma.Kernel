using System.Linq.Expressions;
using Gamma.Kernel.Data;
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Dapper;

public static class SelectBuilder
{
    public static string BuildSelect<TEntity, TProp>(Expression<Func<TEntity, TProp>> expression,
        string targetAlias,
        string? tableAlias = null)
        where TEntity : BaseEntity
    {
        var sourceColumn = ExpressionHelper.GetPropertyName(expression);
        tableAlias ??= BaseSql<TEntity>.TableAlias;
        SqlDialectResolver.EscapeDefaultIdentifier(tableAlias);

        return $"{tableAlias}.[{sourceColumn}] [{targetAlias}]";
    }
}
