using Gamma.Generator;
using Gamma.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gamma.Generator.Services;

public class EntityParser
{
    public EntityInfo Parse(string entityPath)
    {
        var code = File.ReadAllText(entityPath);
        var syntax = CSharpSyntaxTree.ParseText(code);
        var root = syntax.GetCompilationUnitRoot();

        var info = new EntityInfo();

        var classNode = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault()
            ?? throw new Exception("Entity class not found.");

        info.Entity = classNode.Identifier.Text;

        var properties = classNode.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(p => p.Modifiers.Any(SyntaxKind.PublicKeyword));

        foreach (var prop in properties)
        {
            var type = prop.Type.ToString();
            var name = prop.Identifier.Text;

            info.AllProperties.Add((type, name));
        }

        return info;
    }
}