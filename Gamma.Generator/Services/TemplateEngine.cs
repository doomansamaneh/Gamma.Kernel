namespace Gamma.Generator.Services;

public static class TemplateEngine
{
    public static string Render(string template, Dictionary<string, string> values)
    {
        foreach (var pair in values)
        {
            template = template.Replace($"{{{{{pair.Key}}}}}", pair.Value);
        }

        return template;
    }
}

