using Gamma.Kernel.Enums;

namespace Gamma.Kernel.Paging;

public sealed record FilterExpression(string Field, SqlOperator Operator, object? Value);
