namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Dtos;

public sealed record {{Entity}}EditDto
{
    public Guid Id {get; init;}
{{CtorParameters}}
    public long RowVersion {get; init;}
}
