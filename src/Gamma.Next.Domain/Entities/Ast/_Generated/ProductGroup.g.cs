#nullable enable
using System;

namespace Gamma.Next.Domain.Entities.Ast;

public partial class ProductGroup
{
    public static ProductGroup Create(
        long rowNo,
        string code,
        string title,
        string? comment,
        bool isActive
    )
    {
        var entity = new ProductGroup
        {
            RowNo = rowNo,
            Code = code,
            Title = title,
            Comment = comment,
            IsActive = isActive,
            RowVersion = 1
        };

        return entity;
    }

    public void Update(
        long rowNo,
        string code,
        string title,
        string? comment,
        bool isActive,
        long rowVersion
    )
    {
        RowNo = rowNo;
        Code = code;
        Title = title;
        Comment = comment;
        IsActive = isActive;
        RowVersion = rowVersion;
    }
}
