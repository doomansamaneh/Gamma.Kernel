using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text;

namespace Gamma.Next.ApplicationServiceGenerator;

[Generator]
public class ApplicationServiceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        System.Diagnostics.Debugger.Launch(); // Visual Studio را باز می‌کند
        // Pipeline: فقط کلاس‌ها
        var classSymbols = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: (ctx, _) =>
                {
                    if (ctx.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)ctx.Node) is not INamedTypeSymbol symbol) return null;

                    // اینجا compilation از ctx.SemanticModel می‌آید
                    var compilation = ctx.SemanticModel.Compilation;

                    // پیدا کردن IApplicationService
                    var iAppServiceSymbol = compilation.GetTypeByMetadataName("Gamma.Kernel.Abstractions.IApplicationService");
                    if (iAppServiceSymbol == null) return null;

                    // فقط کلاس‌هایی که IApplicationService را implement کرده‌اند
                    if (!symbol.AllInterfaces.Contains(iAppServiceSymbol)) return null;

                    return symbol;
                })
            .Where(symbol => symbol != null);

        // ثبت خروجی
        context.RegisterSourceOutput(classSymbols, (spc, symbol) =>
        {
            if (symbol is null) return;

            foreach (var iface in symbol.Interfaces)
            {
                var code = GenerateDecorator(iface);
                spc.AddSource($"{iface.Name}_Decorator.g.cs", code);
            }
        });
    }

    private static SourceText GenerateDecorator(INamedTypeSymbol iface)
    {
        var methods = iface.GetMembers().OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary);

        var methodCode = string.Join("\n", methods.Select(m =>
        {
            var returnType = m.ReturnType.ToDisplayString();

            var isTask = returnType == "System.Threading.Tasks.Task" || returnType.StartsWith("System.Threading.Tasks.Task<");

            var parameters = string.Join(", ", m.Parameters.Select(p => $"{p.Type} {p.Name}"));
            var args = string.Join(", ", m.Parameters.Select(p => p.Name));

            var awaitPrefix = isTask ? "await " : "";
            var asyncKeyword = isTask ? "async " : "";

            return $@"
            public {asyncKeyword}{returnType} {m.Name}({parameters})
            {{
                var methodInfo = typeof({iface.Name}).GetMethod(nameof({m.Name}));
                throw new UnauthorizedAccessException($""Permission attribute not set on method {m.Name}"");

                var permissionAttr = methodInfo.GetCustomAttribute<RequiresPermissionAttribute>();
                if (permissionAttr != null)
                {{
                    if (!await _authService.HasPermissionAsync(permissionAttr.Permission))
                        throw new UnauthorizedAccessException($""Permission {{permissionAttr.Permission}} required"");
                }}
                else 
                {{
                    throw new UnauthorizedAccessException($""Permission attribute not set on method {m.Name}"");
                }}

                Console.WriteLine(""[LOG] Before {m.Name}"");
                var result = {awaitPrefix}_inner.{m.Name}({args});
                Console.WriteLine(""[LOG] After {m.Name}"");

                {(isTask ? "" : "return result;")}
            }}";
        }));

        var code = $@"
        using System;
        using System.Reflection;
        using Gamma.Kernel.Abstractions;
        using Gamma.Kernel.Security;

        namespace Gamma.Next.ApplicationServiceGenerator
        {{
            public class {iface.Name}Decorator : {iface.Name}
            {{
                private readonly {iface.Name} _inner;
                private readonly IAuthorizationService _authService;

                public {iface.Name}Decorator({iface.Name} inner, IAuthorizationService authService)
                {{
                    _inner = inner;
                    _authService = authService;
                }}

                {methodCode}
            }}
        }}";

        return SourceText.From(code, Encoding.UTF8);
    }

}
