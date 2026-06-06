using System.Text;
using System.CodeDom.Compiler;
using Gamma.Generator;

namespace Gamma.Generator.Models;

public class EntityInfo
{
    public const string indent = "    ";

    public string Schema { get; set; } = "";
    public string Entity { get; set; } = "";

    public string? CreateMethodParameters { get; private set; }
    public string? CreateMethodAssignments { get; private set; }
    public string? UpdateMethodParameters { get; private set; }
    public string? UpdateMethodAssignments { get; private set; }
    public string? EditDtoParameters { get; private set; }
    public string? GridDtoParameters { get; private set; }
    public string? LookupDtoParameters { get; private set; }

    public List<(string Type, string Name)> AllProperties { get; set; } = [];

    public List<(string Type, string Name)> CreateParameters =>
        [.. AllProperties.Where(p => p.Name != "Id")];

    public List<(string Type, string Name)> UpdateParameters =>
        [.. AllProperties.Where(p => p.Name != "Id")];

    // --------------------------------------------------------------
    //  INTERFACE
    // --------------------------------------------------------------

    public string InterfaceProperties =>
        string.Join("\n",
            AllProperties.Select(p =>
                $"{indent}{p.Type} {p.Name} {{ get; }}"));

    // --------------------------------------------------------------
    //  COMMAND PARAMETERS
    // --------------------------------------------------------------

    public string CreateCommandParameters =>
        string.Join(",\n",
            CreateParameters.Select(p =>
                $"{indent}{p.Type} {p.Name}"));

    public string UpdateCommandParameters =>
        string.Join(",\n",
            UpdateParameters
                .Where(p => !p.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                .Select(p => $"{indent}{p.Type} {p.Name}"));

    // Naming parameters for Entity.Create / Entity.Update
    public string CreateNamedParameters =>
        string.Join(",\n",
            CreateParameters.Select(p =>
                $"{indent}{indent}{indent}{p.Name.ToCamel()}: command.{p.Name}"));

    public string UpdateNamedParameters =>
        string.Join(",\n",
            UpdateParameters.Select(p =>
                $"{indent}{indent}{indent}{p.Name.ToCamel()}: command.{p.Name}"));

    // --------------------------------------------------------------
    //  VALIDATION RULE SET
    // --------------------------------------------------------------

    public string SharedValidatorRules =>
        string.Join($"\n\n{indent}",
            AllProperties
                .Where(p => p.Name != "Id")
                .Where(p => !IsNavigationProperty(p.Type))
                .Select(BuildRuleForProperty)
                .Where(rule => !string.IsNullOrWhiteSpace(rule)));

    private string BuildRuleForProperty((string Type, string Name) p)
    {
        var prop = p.Name;
        var type = p.Type;

        if (type == "string")
        {
            return $"{indent}RuleFor(x => x.{prop})\n" +
                   $"{indent}{indent}{indent}.NotEmpty()\n" +
                   $"{indent}{indent}{indent}.WithErrorCode(ValidationCodes.Required)\n" +
                   $"{indent}{indent}{indent}.MaximumLength(240)\n" +
                   $"{indent}{indent}{indent}.WithErrorCode(ValidationCodes.MaxLength);";
        }

        if (type == "string?")
        {
            return $"{indent}RuleFor(x => x.{prop})\n" +
                   $"{indent}{indent}{indent}.MaximumLength(240)\n" +
                   $"{indent}{indent}{indent}.WithErrorCode(ValidationCodes.MaxLength);";
        }

        if (type == "Guid")
        {
            return $"{indent}RuleFor(x => x.{prop})\n" +
                   $"{indent}{indent}{indent}.NotEmpty()\n" +
                   $"{indent}{indent}{indent}.WithErrorCode(ValidationCodes.Required);";
        }

        return string.Empty;
    }

    private static bool IsNavigationProperty(string type)
    {
        return type.StartsWith("ICollection")
            || type.EndsWith('>')
            || type.Contains('.');
    }

    // ==============================================================
    //  DTO PARAMETER GENERATION
    // ==============================================================

    public void BuildDtoParameters()
    {
        EditDtoParameters = BuildDto(AllProperties);

        GridDtoParameters = BuildDto(
            [.. AllProperties.Where(p =>
                            p.Name is "Id" or "Code" or "Title" or "Name" or "Comment" or "IsActive")]);

        LookupDtoParameters = BuildDto(
            [.. AllProperties.Where(p =>
                            p.Name is "Id" or "Code" or "Title" or "Name")]);
    }

    private string BuildDto(List<(string Type, string Name)> props)
    {
        var sb = new StringBuilder();
        foreach (var p in props)
        {
            var type = p.Type; // Already resolved to C# name

            if (type == "string")
            {
                sb.AppendLine($"{indent}public string {p.Name} {{ get; init; }} = default!;");
            }
            else
            {
                sb.AppendLine($"{indent}public {type} {p.Name} {{ get; init; }}");
            }
        }
        return sb.ToString();
    }

    // ==============================================================
    //  SEARCH PROPERTIES (NEW SECTION)
    // ==============================================================

    public List<(string Type, string Name)> SearchProperties { get; private set; } = [];
    public List<(string Type, string Name)> SearchTermProperties { get; private set; } = [];

    public void BuildSearchProperties()
    {
        // دقیق (Guid/Id/Status/IsX)
        SearchProperties =
        [
            .. AllProperties.Where(p =>
                p.Type == "Guid" ||
                p.Type == "Guid?" ||
                p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) ||
                p.Name.StartsWith("Is", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains("Status", StringComparison.OrdinalIgnoreCase))
        ];

        // SearchTerm
        SearchTermProperties =
        [
            .. AllProperties.Where(p =>
                p.Type.StartsWith("string", StringComparison.OrdinalIgnoreCase) &&
                (
                    p.Name.Contains("Name", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.Contains("Title", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.Contains("Code", StringComparison.OrdinalIgnoreCase)
                ))
        ];
    }

    public void BuildEntityMethodParameters()
    {
        var props = AllProperties
            // .Where(p => p.Name != "Id" &&
            //             p.Name != "RowVersion" &&
            //             p.Name != "CreatedAt" &&
            //             p.Name != "UpdatedAt")
            .ToList();

        // ---------- Create ----------
        CreateMethodParameters = string.Join($",\n",
            props.Select(p => $"{indent}{indent}{p.Type} {p.Name.ToCamel()}"));

        CreateMethodAssignments = string.Join($"\n",
            props.Select(p => $"{indent}{indent}{indent}{p.Name} = {p.Name.ToCamel()},"));

        // ---------- Update ----------
        var updateParams = props
            .Select(p => $"{indent}{indent}{p.Type} {p.Name.ToCamel()}")
            .ToList();

        UpdateMethodParameters = string.Join($",\n", updateParams);

        UpdateMethodAssignments = string.Join($"\n",
            props.Select(p => $"{indent}{indent}{p.Name} = {p.Name.ToCamel()};"));
    }

}
