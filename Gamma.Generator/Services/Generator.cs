using System.Linq;
using System.Runtime.CompilerServices;
using Gamma.Generator.Models;

namespace Gamma.Generator.Services;

public class Generator
{
    private readonly string _apiPath;
    private readonly string _domainPath;
    private readonly string _applicationPath;
    private readonly string _infraPath;
    private readonly string _baseNamespace;

    public Generator(string domainPath,
        string applicationPath,
        string apiPath,
        string infraPath,
        string baseNamespace)
    {
        _domainPath = domainPath;
        _applicationPath = applicationPath;
        _apiPath = apiPath;
        _infraPath = infraPath;
        _baseNamespace = baseNamespace;
    }

    public void Generate(EntityInfo info)
    {
        var values = new Dictionary<string, string>
        {
            {"Entity", info.Entity},
            {"Schema", info.Schema},

            {"schema_lower", info.Schema.ToLower()},
            {"entity_lower", info.Entity.ToLower()},

            {"DomainNamespace", $"{_baseNamespace}.Domain"},
            {"ApplicationNamespace", $"{_baseNamespace}.Application"},
            {"InfraNamespace", $"{_baseNamespace}.Infra"},
            {"ApiNamespace", $"{_baseNamespace}.Api"},

            { "CreateCommandParameters", info.CreateCommandParameters ?? string.Empty },
            { "CreateNamedParameters", info.CreateNamedParameters ?? string.Empty },
            { "UpdateCommandParameters", info.UpdateCommandParameters ?? string.Empty },
            { "UpdateNamedParameters", info.UpdateNamedParameters ?? string.Empty },

            { "InterfaceProperties", info.InterfaceProperties ?? string.Empty},

            { "SharedValidatorRules", info.SharedValidatorRules ?? string.Empty},
        };

        GenerateDomain(info, values);
        GenerateApplication(info, values);
        GenerateApi(info, values);
        GenerateInfra(info, values);
    }


    #region Domain
    private void GenerateDomain(EntityInfo info, Dictionary<string, string> values)
    {
        GenerateEntityMethods(info, values);
        GenerateInterfaces(info, values);
    }

    private void GenerateInterfaces(EntityInfo info, Dictionary<string, string> baseValues)
    {
        var outputFile = Path.Combine(_domainPath, $"Interfaces/{info.Schema}/I{info.Entity}Repository.cs");
        GenerateFile("Templates/Domain/IRepository.tpl", outputFile, baseValues);
    }
    private void GenerateEntityMethods(EntityInfo info, Dictionary<string, string> baseValues)
    {
        info.BuildEntityMethodParameters();

        // -------------------------
        // پارامترها و assignment ها
        // -------------------------

        info.BuildEntityMethodParameters();

        var values = new Dictionary<string, string>(baseValues)
        {
            { "CreateParameters", info.CreateMethodParameters ?? "" },
            { "CreateAssignments", info.CreateMethodAssignments ?? "" },

            { "UpdateParameters", info.UpdateMethodParameters ?? "" },
            { "UpdateAssignments", info.UpdateMethodAssignments ?? "" }
        };

        var outputFile = Path.Combine(_domainPath, $"Entities/{info.Schema}/_Generated/{info.Entity}.g.cs");
        GenerateFile("Templates/Domain/EntityMethods.tpl", outputFile, values);
    }
    #endregion Domain

    #region Application
    private void GenerateApplication(EntityInfo info, Dictionary<string, string> values)
    {
        GenerateCommands(info, values);
        GenerateDtos(info, values);
        GenerateQueries(info, values);
        GenerateSql(info, values);
    }
    private void GenerateCommands(EntityInfo info, Dictionary<string, string> values)
    {
        var commandsPath = Path.Combine(_applicationPath, "Commands");
        Directory.CreateDirectory(commandsPath);

        GenerateFile("Templates/Commands/CreateCommand.tpl",
                    Path.Combine(commandsPath, $"Create{info.Entity}.cs"),
                    values);

        GenerateFile("Templates/Commands/UpdateCommand.tpl",
                    Path.Combine(commandsPath, $"Update{info.Entity}.cs"),
                    values);

        GenerateFile("Templates/Commands/DeleteCommand.tpl",
                    Path.Combine(commandsPath, $"Delete{info.Entity}.cs"),
                    values);

        GenerateFile("Templates/Commands/Shared.tpl",
                    Path.Combine(commandsPath, $"Shared.cs"),
                    values);

        GenerateFile("Templates/Commands/DeleteBatchCommand.tpl",
                    Path.Combine(commandsPath, $"DeleteBatch{info.Entity}.cs"),
                    values);

        if (info.AllProperties.Any(p => p.Name == "IsActive"))
        {
            GenerateFile("Templates/Commands/ActivateCommand.tpl",
                        Path.Combine(commandsPath, $"Activate{info.Entity}.cs"),
                        values);
        }
    }
    private void GenerateDtos(EntityInfo info, Dictionary<string, string> baseValues)
    {
        var dtosPath = Path.Combine(_applicationPath, "Dtos");
        Directory.CreateDirectory(dtosPath);

        info.BuildDtoParameters();

        // EditDto
        var editValues = new Dictionary<string, string>(baseValues)
        {
            { "CtorParameters", info.EditDtoParameters ?? string.Empty }
        };
        GenerateFile("Templates/Dtos/EditEntityDto.tpl",
            Path.Combine(dtosPath, $"{info.Entity}EditDto.cs"),
            editValues);

        // GridDto
        var gridValues = new Dictionary<string, string>(baseValues)
        {
            { "DtoSuffix", "GridDto" },
            { "CtorParameters", info.GridDtoParameters ?? string.Empty }
        };
        GenerateFile("Templates/Dtos/GridDto.tpl",
            Path.Combine(dtosPath, $"{info.Entity}GridDto.cs"),
            gridValues);

        // LookupDto
        var lookupValues = new Dictionary<string, string>(baseValues)
        {
            { "DtoSuffix", "LookupDto" },
            { "CtorParameters", info.LookupDtoParameters ?? string.Empty }
        };
        GenerateFile("Templates/Dtos/GridDto.tpl",
            Path.Combine(dtosPath, $"{info.Entity}LookupDto.cs"),
            lookupValues);

        // SearchDto
        GenerateFile("Templates/Dtos/SearchDto.tpl",
            Path.Combine(dtosPath, $"{info.Entity}SearchDto.cs"),
            baseValues);
    }

    private void GenerateQueries(EntityInfo info, Dictionary<string, string> values)
    {
        var queriesPath = Path.Combine(_applicationPath, "Queries");
        Directory.CreateDirectory(queriesPath);

        GenerateFile("Templates/Queries/GetByIdQuery.tpl",
            Path.Combine(queriesPath, $"{info.Entity}ByIdQuery.cs"),
            values);

        GenerateFile("Templates/Queries/GetGridQuery.tpl",
            Path.Combine(queriesPath, $"{info.Entity}GridQuery.cs"),
            values);

        GenerateFile("Templates/Queries/GetLookupQuery.tpl",
            Path.Combine(queriesPath, $"{info.Entity}LookupQuery.cs"),
            values);

        GenerateFile("Templates/Queries/ExportQuery.tpl",
            Path.Combine(queriesPath, $"{info.Entity}ExportQuery.cs"),
            values);
    }

    private void GenerateSql(EntityInfo info, Dictionary<string, string> baseValues)
    {
        info.BuildSearchProperties();

        var sqlPath = Path.Combine(_applicationPath, "Sql");
        Directory.CreateDirectory(sqlPath);

        // Build dynamic SQL blocks
        var alias = info.Entity.Substring(0, 1).ToLower();

        var baseQuerySelects = string.Join("\n",
            info.AllProperties.Select(p =>
                $"{EntityInfo.indent}{EntityInfo.indent}{EntityInfo.indent}{EntityInfo.indent}.Select($\"{{TableAlias}}.[{{nameof(Domain.Entities.{info.Schema}.{info.Entity}.{p.Name})}}]\")"));


        var getByIdSelects = string.Join("\n",
            info.AllProperties.Select(p =>
                $"{EntityInfo.indent}{EntityInfo.indent}{EntityInfo.indent}.Select($\"{{TableAlias}}.[{{nameof(Domain.Entities.{info.Schema}.{info.Entity}.{p.Name})}}]\")"));

        var searchConditions = string.Join("\n\n",
            info.SearchProperties.Select(p => BuildSearchCondition(info, p)));

        // var searchTermConditions = string.Join(
        //     $"\n{EntityInfo.indent}\n",
        //     info.SearchTermProperties.Select(p =>
        //         $"{EntityInfo.indent}{EntityInfo.indent}{{TableAlias}}.[{{nameof(Domain.Entities.{info.Schema}.{info.Entity}.{p.Name})}}]"
        //     ));
        var searchTermConditions = string.Join(
            ",\n",
            info.SearchTermProperties.Select(p =>
                $"{EntityInfo.indent}{EntityInfo.indent}{EntityInfo.indent}$\"{{TableAlias}}.[{{nameof(Domain.Entities.{info.Schema}.{info.Entity}.{p.Name})}}]\""
            ));

        var columnMapping = string.Join(
            ",\n",
            info.AllProperties.Select(p =>
                $@"{EntityInfo.indent}{EntityInfo.indent}{EntityInfo.indent}[nameof(Domain.Entities.{info.Schema}.{info.Entity}.{p.Name})] = $""{{TableAlias}}.[{{nameof(Domain.Entities.{info.Schema}.{info.Entity}.{p.Name})}}]"""
            ));

        var lookupCondition = info.AllProperties.Any(p => p.Name == "IsActive")
            ? $"""
            {EntityInfo.indent}sql.Where<Domain.Entities.{info.Schema}.{info.Entity}, bool>(x => x.IsActive, SqlOperator.Equals, "@isActive");
            {EntityInfo.indent}{EntityInfo.indent}sql.AddParameter("isActive", true);
            """
            : string.Empty;


        var sqlValues = new Dictionary<string, string>(baseValues)
        {
            { "alias", alias },
            { "BaseQuerySelects", baseQuerySelects },
            { "GetByIdSelects", getByIdSelects },
            { "SearchConditions", searchConditions },
            { "SearchTermConditions", searchTermConditions },
            { "ColumnMapping", columnMapping },
            { "LookupCondition", lookupCondition },
        };

        GenerateFile("Templates/Sql/EntitySql.tpl",
            Path.Combine(sqlPath, $"{info.Entity}Sql.cs"),
            sqlValues);
    }
    private string BuildSearchCondition(EntityInfo info, (string Type, string Name) p)
    {
        return
            $@"{EntityInfo.indent}if (!string.IsNullOrWhiteSpace(search.{p.Name}))
            {EntityInfo.indent}{{
            {EntityInfo.indent}{EntityInfo.indent}sql.Where($""{{TableAlias}}.{{nameof(Domain.Entities.{info.Schema}.{info.Entity}.{p.Name})}} LIKE @{{nameof(search.{p.Name})}}"");
            {EntityInfo.indent}{EntityInfo.indent}sql.AddParameter(nameof(search.{p.Name}), $""%{{search.{p.Name}}}%"");
            {EntityInfo.indent}}}";
    }
    #endregion Application

    private void GenerateApi(EntityInfo info, Dictionary<string, string> values)
    {
        GenerateFile("Templates/Api/Endpoint.tpl",
            Path.Combine(_apiPath, $"{info.Entity}Endpoints.cs"),
            values);
    }

    #region Infra
    private void GenerateInfra(EntityInfo info, Dictionary<string, string> values)
    {
        GenerateFile("Templates/Infra/Repository.tpl",
            Path.Combine(_infraPath, $"Repositories/{info.Schema}/{info.Entity}Repository.cs"),
            values);
    }
    #endregion Infra

    private void GenerateFile(string templatePath,
                              string fileName,
                              Dictionary<string, string> values)
    {
        var template = File.ReadAllText(templatePath);

        var result = TemplateEngine.Render(template, values);

        var path = Path.Combine(_applicationPath, fileName);

        File.WriteAllText(path, result);
    }
}
