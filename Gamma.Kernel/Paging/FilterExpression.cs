using Gamma.Kernel.Enums;

namespace Gamma.Kernel.Paging;

public sealed record FilterExpression(string FieldName, SqlOperator Operator, object? Value);
