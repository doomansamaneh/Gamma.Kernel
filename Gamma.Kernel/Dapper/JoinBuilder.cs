using System.Linq.Expressions;
using Gamma.Kernel.Data;
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Dapper;

public static class JoinBuilder
{
    public static void Inner<TLeft, TRight, TKey>(SqlBuilder sql,
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

        var leftColumn = ExpressionHelper.GetPropertyName(left);
        var rightColumn = ExpressionHelper.GetPropertyName(right);

        var leftAlias = string.IsNullOrWhiteSpace(leftTableAlias)
            ? BaseSql<TLeft>.TableAlias
            : leftTableAlias;
        leftAlias = SqlDialectResolver.EscapeDefaultIdentifier(leftAlias);

        var rightAlias = string.IsNullOrWhiteSpace(rightTableAlias)
            ? BaseSql<TRight>.TableAlias
            : rightTableAlias;
        rightAlias = SqlDialectResolver.EscapeDefaultIdentifier(rightAlias);

        var rightTable = BaseSql<TRight>.TableName;

        sql.InnerJoin(
            $"{rightTable} {rightAlias} ON " +
            $"{rightAlias}.[{rightColumn}] = {leftAlias}.[{leftColumn}]"
        );
    }

    public static void Left<TLeft, TRight, TKey>(SqlBuilder sql,
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

        var leftColumn = ExpressionHelper.GetPropertyName(left);
        var rightColumn = ExpressionHelper.GetPropertyName(right);

        var leftAlias = string.IsNullOrWhiteSpace(leftTableAlias)
            ? BaseSql<TLeft>.TableAlias
            : leftTableAlias;
        leftAlias = SqlDialectResolver.EscapeDefaultIdentifier(leftAlias);

        var rightAlias = string.IsNullOrWhiteSpace(rightTableAlias)
            ? BaseSql<TRight>.TableAlias
            : rightTableAlias;
        rightAlias = SqlDialectResolver.EscapeDefaultIdentifier(rightAlias);

        var rightTable = BaseSql<TRight>.TableName;

        sql.LeftJoin(
            $"{rightTable} {rightAlias} ON " +
            $"{rightAlias}.[{rightColumn}] = {leftAlias}.[{leftColumn}]"
        );
    }

}
