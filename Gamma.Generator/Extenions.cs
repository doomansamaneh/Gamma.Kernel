namespace Gamma.Generator;

public static class Extenions
{
    public static string ToPascal(this string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        return char.ToUpper(s[0]) + s[1..];
    }

    public static string ToCamel(this string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        return char.ToLower(s[0]) + s[1..];
    }
}