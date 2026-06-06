#nullable enable
using System;

namespace Gamma.Next.Domain.Entities.Ast;

public partial class Product
{
    public static Product Create(
        Guid productGroupId,
        string code,
        string title,
        string? comment,
        bool isActive
    )
    {
        var entity = new Product
        {
            ProductGroupId = productGroupId,
            Code = code,
            Title = title,
            Comment = comment,
            IsActive = isActive,
            RowVersion = 1
        };

        return entity;
    }

    public void Update(
        Guid productGroupId,
        string code,
        string title,
        string? comment,
        bool isActive,
        long rowVersion
    )
    {
        ProductGroupId = productGroupId;
        Code = code;
        Title = title;
        Comment = comment;
        IsActive = isActive;
        RowVersion = rowVersion;
    }
}
