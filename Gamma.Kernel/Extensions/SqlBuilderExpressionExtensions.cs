using Gamma.Kernel.Dapper;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Data;
using System.Linq.Expressions;
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Extensions;

public static class SqlBuilderExpressionExtensions
{
    public static SqlBuilder InnerJoin<TLeft, TRight>(this SqlBuilder sql,
        Expression<Func<TLeft, object>> left,
        Expression<Func<TRight, object>> right,
        string? rightTableAlias = null,
        string? leftTableAlias = null)
        where TLeft : BaseEntity
        where TRight : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(sql);
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        JoinBuilder.Inner(sql, left, right, rightTableAlias, leftTableAlias);
        return sql;
    }

    public static SqlBuilder InnerJoin<TLeft, TRight, TKey>(this SqlBuilder sql,
        Expression<Func<TLeft, TKey>> left,
        Expression<Func<TRight, TKey>> right,
        string? rightTableAlias = null,
        string? leftTableAlias = null)
        where TLeft : BaseEntity
        where TRight : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(sql);
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        JoinBuilder.Inner(sql, left, right, rightTableAlias, leftTableAlias);
        return sql;
    }

    public static SqlBuilder LeftJoin<TLeft, TRight, TKey>(this SqlBuilder sql,
        Expression<Func<TLeft, TKey>> left,
        Expression<Func<TRight, TKey>> right,
        string? rightTableAlias = null,
        string? leftTableAlias = null)
        where TLeft : BaseEntity
        where TRight : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(sql);
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        JoinBuilder.Left(sql, left, right, rightTableAlias, leftTableAlias);
        return sql;
    }

    public static SqlBuilder Select<TEntity, TProp>(this SqlBuilder sql,
        Expression<Func<TEntity, TProp>> expr,
        string? targetAlias = null,
        string? tableAlias = null)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(sql);
        ArgumentNullException.ThrowIfNull(expr);

        var propertyName = ExpressionHelper.GetPropertyName(expr);
        if (string.IsNullOrWhiteSpace(targetAlias))
            targetAlias = propertyName;

        var columnSql = SelectBuilder.BuildSelect(expr, targetAlias, tableAlias);
        sql.Select(columnSql);
        return sql;
    }

    public static SqlBuilder Select<TEntity>(this SqlBuilder sql,
        Expression<Func<TEntity, object?>> expr,
        string? targetAlias = null,
        string? tableAlias = null)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(sql);
        ArgumentNullException.ThrowIfNull(expr);

        var propertyName = ExpressionHelper.GetPropertyName(expr);
        if (string.IsNullOrWhiteSpace(targetAlias)) targetAlias = propertyName;

        var columnSql = SelectBuilder.BuildSelect(expr, targetAlias, tableAlias);
        sql.Select(columnSql);
        return sql;
    }

    public static SqlBuilder From<TEntity>(this SqlBuilder sql,
        string? alias = null)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(sql);

        var tableName = BaseSql<TEntity>.TableName;

        alias = string.IsNullOrWhiteSpace(alias)
            ? BaseSql<TEntity>.TableAlias
            : SqlDialectResolver.EscapeDefaultIdentifier(alias);

        sql.From($"{tableName} {alias}");
        return sql;
    }

    public static SqlBuilder Where<TEntity, TProp>(this SqlBuilder sql,
        Expression<Func<TEntity, TProp>> column,
        SqlOperator op,
        string? parameter = null,
        string? alias = null)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(sql);
        ArgumentNullException.ThrowIfNull(column);

        var columnName = ExpressionHelper.GetPropertyName(column);
        var resolvedAlias = string.IsNullOrWhiteSpace(alias)
            ? BaseSql<TEntity>.TableAlias
            : alias;

        var left = $"{resolvedAlias}.[{columnName}]";

        var clause = op switch
        {
            SqlOperator.Equals => $"{left} = {Require(parameter)}",
            SqlOperator.NotEquals => $"{left} <> {Require(parameter)}",
            SqlOperator.LessThan => $"{left} < {Require(parameter)}",
            SqlOperator.LessThanOrEqual => $"{left} <= {Require(parameter)}",
            SqlOperator.GreaterThan => $"{left} > {Require(parameter)}",
            SqlOperator.GreaterThanOrEqual => $"{left} >= {Require(parameter)}",
            SqlOperator.In => $"{left} IN {Require(parameter)}",
            SqlOperator.NotIn => $"{left} NOT IN {Require(parameter)}",
            SqlOperator.Contains => $"{left} LIKE {Require(parameter)}",
            SqlOperator.NotContains => $"{left} NOT LIKE {Require(parameter)}",
            SqlOperator.StartsWith => $"{left} LIKE {Require(parameter)}",
            SqlOperator.EndsWith => $"{left} LIKE {Require(parameter)}",
            SqlOperator.IsNull => $"{left} IS NULL",
            SqlOperator.IsNotNull => $"{left} IS NOT NULL",
            _ => throw new NotSupportedException($"SqlOperator '{op}' is not supported.")
        };

        sql.Where(clause);
        return sql;
    }

    private static string Require(string? parameter) =>
        string.IsNullOrWhiteSpace(parameter)
            ? throw new ArgumentException("Parameter is required.")
            : parameter;
}
