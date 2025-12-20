using System.Dynamic;
using Gamma.Kernel.Enums;
using Gamma.Kernel.Extensions;

namespace Gamma.Kernel.Dapper;

public sealed class SqlBuilder
{
    public const string WithNoLock = "WITH (NOLOCK)";

    private readonly SortedDictionary<SqlClause, ClauseBuffer> _buffers = [];

    public object? Parameters { get; private set; }

    #region Fluent API

    public SqlBuilder Select(string sql) =>
        Append(SqlClause.Select, "SELECT ", ", ", sql, allowDuplicate: true);

    public SqlBuilder From(string sql) =>
        Append(SqlClause.From, " FROM ", null, sql);

    public SqlBuilder InnerJoin(string sql) =>
        Append(SqlClause.Join, " INNER JOIN ", null, sql);

    public SqlBuilder LeftJoin(string sql) =>
        Append(SqlClause.Join, " LEFT JOIN ", null, sql);

    public SqlBuilder RightJoin(string sql) =>
        Append(SqlClause.Join, " RIGHT JOIN ", null, sql);

    public SqlBuilder Where(string sql) =>
        Append(SqlClause.Where, " WHERE ", " AND ", sql);

    public SqlBuilder GroupBy(string sql) =>
        Append(SqlClause.GroupBy, " GROUP BY ", ", ", sql);

    public SqlBuilder Having(string sql) =>
        Append(SqlClause.Having, " HAVING ", " AND ", sql);

    public SqlBuilder OrderBy(string sql) =>
        Append(SqlClause.OrderBy, " ORDER BY ", ", ", sql);

    public SqlBuilder Update(string sql) =>
        Append(SqlClause.Update, "UPDATE ", null, sql);

    public SqlBuilder DeleteFrom(string sql) =>
        Append(SqlClause.DeleteFrom, "DELETE FROM ", null, sql);

    public SqlBuilder Set(string sql) =>
        Append(SqlClause.Set, " SET ", ", ", sql);

    public SqlBuilder If(bool condition, Func<SqlBuilder, SqlBuilder> action)
    {
        if (condition) action(this);
        return this;
    }

    #endregion

    #region Core Append Logic

    private SqlBuilder Append(
        SqlClause clause,
        string prefix,
        string? separator,
        string sql,
        bool allowDuplicate = false)
    {
        if (!_buffers.TryGetValue(clause, out var buffer))
        {
            buffer = new ClauseBuffer(prefix, separator);
            _buffers.Add(clause, buffer);
        }

        if (allowDuplicate || !buffer.Contains(sql))
        {
            buffer.Append(sql);
        }

        return this;
    }

    #endregion

    #region Parameters

    public SqlBuilder AddParameter(string name, object value)
    {
        if (string.IsNullOrWhiteSpace(name))
            return this;

        var dict = Parameters as IDictionary<string, object?> ?? new ExpandoObject();

        if (!dict.ContainsKey(name))
            dict[name] = value;

        Parameters = dict;
        return this;
    }

    #endregion

    #region Output Helpers

    public override string ToString() =>
        string.Join(" ", _buffers.Values.Select(b => b.Build()))
              .NormalizeWhiteSpace();

    public string WithoutOrderBy() =>
        BuildExcluding(SqlClause.OrderBy);

    public string WithoutSelectAndOrderBy() =>
        BuildExcluding(SqlClause.Select, SqlClause.OrderBy);

    public string GetWhereCondition() =>
        GetClause(SqlClause.Where)?.Replace("WHERE", "", StringComparison.OrdinalIgnoreCase)
        ?? string.Empty;

    public string GetOrderBy() =>
        GetClause(SqlClause.OrderBy)?.Replace("ORDER BY", "", StringComparison.OrdinalIgnoreCase)
        ?? string.Empty;

    public string GetJoinClause() =>
        GetClause(SqlClause.Join) ?? string.Empty;

    private string BuildExcluding(params SqlClause[] excluded) =>
        string.Join(" ",
            _buffers
                .Where(b => !excluded.Contains(b.Key))
                .Select(b => b.Value.Build()))
        .NormalizeWhiteSpace();

    private string? GetClause(SqlClause clause) =>
        _buffers.TryGetValue(clause, out var buffer)
            ? buffer.Build()
            : null;

    #endregion

    #region Clone

    public SqlBuilder Clone()
    {
        var clone = new SqlBuilder();
        foreach (var (key, value) in _buffers)
            clone._buffers[key] = value.Clone();

        clone.Parameters = Parameters;
        return clone;
    }

    #endregion
}

internal sealed class ClauseBuffer
{
    private readonly string _prefix;
    private readonly string? _separator;
    private readonly List<string> _items = [];

    public ClauseBuffer(string prefix, string? separator)
    {
        _prefix = prefix;
        _separator = separator;
    }

    public void Append(string sql) => _items.Add(sql);

    public bool Contains(string sql)
        => _items.Any(i => i.Equals(sql, StringComparison.OrdinalIgnoreCase));

    public string Build()
        => _items.Count == 0
            ? string.Empty
            : _prefix + string.Join(_separator ?? string.Empty, _items);

    public ClauseBuffer Clone()
    {
        var clone = new ClauseBuffer(_prefix, _separator);
        clone._items.AddRange(_items);
        return clone;
    }
}

