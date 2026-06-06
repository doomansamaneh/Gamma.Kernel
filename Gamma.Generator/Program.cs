using Gamma.Generator;
using Gamma.Generator.Services;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("gamma-gen org/organization.cs");
            return;
        }

        var input = args[0];

        var parts = input.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            Console.WriteLine("Invalid format.");
            Console.WriteLine("Example: org/organization.cs");
            return;
        }

        var schema = parts[0].ToPascal();
        var entityFile = parts[1];

        var entityName = Path.GetFileNameWithoutExtension(entityFile).ToPascal();

        if (string.IsNullOrWhiteSpace(entityName))
        {
            Console.WriteLine("Invalid entity name.");
            return;
        }

        // ✅ پیدا کردن solution root
        var currentDir = new DirectoryInfo(Environment.CurrentDirectory);

        var solutionRoot = currentDir.Parent;

        if (solutionRoot == null)
        {
            Console.WriteLine("Could not determine solution root.");
            return;
        }

        var solutionRootPath = $"{solutionRoot.FullName}/src";

        // ✅ نام فولدر solution
        var solutionFolder = solutionRoot.Name;

        // gamma.zed -> Gamma.Zed
        var solutionName = string.Join(".",
            solutionFolder
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToPascal()));

        var domainProject = $"{solutionName}.Domain";
        var applicationProject = $"{solutionName}.Application";
        var infraProject = $"{solutionName}.Infra";

        // ✅ مسیر Entity
        var entityPath = Path.Combine(
            solutionRootPath,
            domainProject,
            "Entities",
            schema,
            entityName + ".cs"
        );

        if (!File.Exists(entityPath))
        {
            Console.WriteLine("Entity not found:");
            Console.WriteLine(entityPath);
            return;
        }

        var parser = new EntityParser();
        var info = parser.Parse(entityPath);

        info.Schema = schema;
        info.Entity = entityName;

        // ✅ مسیر خروجی Commands
        var applicationPath = Path.Combine(
            solutionRootPath,
            applicationProject,
            schema,
            entityName
        );

        var domainPath = Path.Combine(
            solutionRootPath,
            domainProject
        );

        var infraPath = Path.Combine(
            solutionRootPath,
            infraProject
        );

        var apiPath = Path.Combine(
            solutionRootPath,
            $"{solutionName}.Api",
            "Endpoints",
            schema
        );

        Directory.CreateDirectory(applicationPath);

        var generator = new Generator(domainPath: domainPath,
            applicationPath: applicationPath,
            apiPath: apiPath,
            infraPath: infraPath,
            baseNamespace: solutionName);
        generator.Generate(info);

        Console.WriteLine();
        Console.WriteLine("✅ Commands Generated Successfully:");
        Console.WriteLine(applicationPath);
    }
}
