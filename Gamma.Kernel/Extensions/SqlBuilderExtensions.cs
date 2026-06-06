using System.Data;
using Gamma.Kernel.Paging;
using Gamma.Kernel.Dapper;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Data;

namespace Gamma.Kernel.Extensions;

public static class SqlBuilderExtensions
{
    public static SqlBuilder ApplyFilters(this SqlBuilder builder,
        IEnumerable<FilterExpression>? filters,
        IReadOnlyDictionary<string, string> columnMapping)
    {
        if (filters is null || !filters.Any())
        {
            return builder;
        }

        var paramIndex = 0;

        foreach (var filter in filters)
        {
            if (string.IsNullOrWhiteSpace(filter.FieldName)) continue;

            if (!columnMapping.TryGetValue(filter.FieldName, out var dbColumnName))
            {
                dbColumnName = filter.FieldName;
            }
            dbColumnName = SqlDialectResolver.EscapeDefaultIdentifier(dbColumnName);

            if (filter.Operator == SqlOperator.IsNull)
            {
                builder.Where($"{dbColumnName} IS NULL");
                continue;
            }

            if (filter.Operator == SqlOperator.IsNotNull)
            {
                builder.Where($"{dbColumnName} IS NOT NULL");
                continue;
            }

            if (filter.Value is null) continue;

            object fieldValue = filter.Value;
            if (filter.Value is string s)
            {
                if (string.IsNullOrWhiteSpace(s)) continue;
                fieldValue = s.ToNormalString();
            }

            // Generate a unique parameter name
            string paramName = $"p__{paramIndex++}";

            switch (filter.Operator)
            {
                case SqlOperator.Equals:
                    builder.Where($"{dbColumnName} = @{paramName}");
                    builder.AddParameter(paramName, $"{fieldValue}");
                    break;
                case SqlOperator.NotEquals:
                    builder.Where($"{dbColumnName} != @{paramName}");
                    builder.AddParameter(paramName, $"{fieldValue}");
                    break;
                case SqlOperator.Contains:
                    builder.Where($"{dbColumnName} LIKE @{paramName}");
                    builder.AddParameter(paramName, $"%{fieldValue}%");
                    break;
                case SqlOperator.NotContains:
                    builder.Where($"{dbColumnName} NOT LIKE @{paramName}");
                    builder.AddParameter(paramName, $"%{fieldValue}%");
                    break;
                case SqlOperator.LessThan:
                    builder.Where($"{dbColumnName} < @{paramName}");
                    builder.AddParameter(paramName, $"{fieldValue}");
                    break;
                case SqlOperator.LessThanOrEqual:
                    builder.Where($"{dbColumnName} <= @{paramName}");
                    builder.AddParameter(paramName, $"{fieldValue}");
                    break;
                case SqlOperator.GreaterThan:
                    builder.Where($"{dbColumnName} > @{paramName}");
                    builder.AddParameter(paramName, $"{fieldValue}");
                    break;
                case SqlOperator.GreaterThanOrEqual:
                    builder.Where($"{dbColumnName} >= @{paramName}");
                    builder.AddParameter(paramName, $"{fieldValue}");
                    break;
                case SqlOperator.In:
                    builder.Where($"{dbColumnName} IN @{paramName}");
                    builder.AddParameter(paramName, $"{fieldValue}");
                    break;
                case SqlOperator.NotIn:
                    builder.Where($"{dbColumnName} NOT IN @{paramName}");
                    builder.AddParameter(paramName, $"{fieldValue}");
                    break;
                case SqlOperator.StartsWith:
                    builder.Where($"{dbColumnName} LIKE @{paramName}");
                    builder.AddParameter(paramName, $"{fieldValue}%");
                    break;
                case SqlOperator.EndsWith:
                    builder.Where($"{dbColumnName} LIKE @{paramName}");
                    builder.AddParameter(paramName, $"%{fieldValue}");
                    break;
                case SqlOperator.None:
                    break;

                default:
                    throw new NotSupportedException($"Operator '{filter.Operator}' is not supported.");
            }
        }

        return builder;
    }

    public static SqlBuilder ApplySort(this SqlBuilder builder,
        string? sortColumn,
        SortOrder sortOrder,
        IReadOnlyDictionary<string, string> columnMapping)
    {
        if (string.IsNullOrWhiteSpace(sortColumn))
        {
            return builder;
        }

        if (!columnMapping.TryGetValue(sortColumn, out var dbColumnName))
        {
            dbColumnName = sortColumn;
        }
        dbColumnName = SqlDialectResolver.EscapeDefaultIdentifier(dbColumnName);
        var sortOrderString = sortOrder == SortOrder.Ascending ? "" : "DESC";
        builder.OrderBy($"{dbColumnName} {sortOrderString}");
        return builder;
    }

    public static SqlBuilder ApplyMultiWordSearch(this SqlBuilder builder,
            string? searchTerm,
            params string[] columnsToSearch)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || columnsToSearch == null || columnsToSearch.Length == 0)
            return builder;

        // split string
        var words = searchTerm.ToNormalString()
                                .Split([' ', '‌'], StringSplitOptions.RemoveEmptyEntries);
        var conditions = new List<string>();

        // create param
        string paramPrefix = $"st_{Guid.NewGuid().ToString("N")[..4]}";

        for (int i = 0; i < words.Length; i++)
        {
            var paramName = $"@{paramPrefix}_{i}";

            // OR
            var columnConditions = columnsToSearch.Select(col => $"{col} LIKE {paramName}");
            conditions.Add($"({string.Join(" OR ", columnConditions)})");

            builder.AddParameter(paramName, $"%{words[i]}%");
        }

        if (conditions.Count != 0)
        {
            builder.Where($"({string.Join(" AND ", conditions)})");
        }

        return builder;
    }
}
