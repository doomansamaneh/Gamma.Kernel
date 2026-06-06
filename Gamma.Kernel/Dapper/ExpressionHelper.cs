using System.Linq.Expressions;
using Gamma.Kernel.Data;
using Gamma.Kernel.Entities;

namespace Gamma.Kernel.Dapper;

internal static class ExpressionHelper
{
    public static string GetPropertyName<T, TProp>(
     Expression<Func<T, TProp>> expression)
    {
        if (expression.Body is MemberExpression m)
            return m.Member.Name;

        if (expression.Body is UnaryExpression u &&
            u.Operand is MemberExpression um)
            return um.Member.Name;

        throw new InvalidOperationException("Invalid expression");
    }
}

