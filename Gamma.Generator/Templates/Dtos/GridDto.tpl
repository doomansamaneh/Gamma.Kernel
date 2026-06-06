namespace {{ApplicationNamespace}}.{{Schema}}.{{Entity}}.Dtos;

public sealed record {{Entity}}{{DtoSuffix}}
{
    public Guid Id {get; init;}
{{CtorParameters}}
}
