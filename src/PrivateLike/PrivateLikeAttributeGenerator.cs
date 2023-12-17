using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PrivateLike.Helpers;
using PrivateLike.Models;

namespace PrivateLike;

/// <summary>
/// The incremental generator producing the <c>[privatelike]</c> attribute type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class PrivateLikeAttributeGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context => context.AddSource("privatelikeAttribute.g.cs",
             """
            /// <summary>
            /// An attribute that can be used to declare types that are "private-ish". That is, they are
            /// <see langword="internal"/>, but they produce an error via an analyzer if they are used
            /// from anywhere they would not be accessible from if they had been <see langword="private"/>.
            /// This allows source generators (who would be suppressing all diagnostics in generated files)
            /// to still refer to the types, but not developers.
            /// </summary>
            [global::System.AttributeUsage(
                global::System.AttributeTargets.Struct |
                global::System.AttributeTargets.Class |
                global::System.AttributeTargets.Interface,
                AllowMultiple = false,
                Inherited = false)]
            internal sealed class privatelikeAttribute : global::System.Attribute
            {
            }
            """));

        IncrementalValuesProvider<HierarchyInfo> structDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
            "privatelikeAttribute",
            static (node, _) => node.Kind() is SyntaxKind.StructDeclaration or SyntaxKind.ClassDeclaration or SyntaxKind.InterfaceDeclaration,
            static (context, _) => HierarchyInfo.From((INamedTypeSymbol)context.TargetSymbol));

        context.RegisterSourceOutput(structDeclarations, static (context, info) =>
        {
            using IndentedTextWriter writer = new();

            info.WriteSyntax(writer);

            context.AddSource($"{info.FullyQualifiedMetadataName}.g.cs", writer.ToString());
        });
    }
}