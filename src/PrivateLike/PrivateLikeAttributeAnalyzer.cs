using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using PrivateLike.Extensions;

namespace PrivateLike;

/// <summary>
/// The diagnostic analyzer for <c>[privatelike]</c> uses.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PrivateLikeAttributeAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The diagnostic for an invalid type reference to a <c>[privatelike]</c> type.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidReferenceToPrivateLikeType = new(
        id: "PRIV0001",
        title: "Invalid reference to private-like type",
        messageFormat: "Type '{0}' cannot be referenced from '{1}', as it's private-like",
        category: "PrivateAttribute",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Private-like types can't be referenced outside of scopes where they would've also been accessible if they had been private.");

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(InvalidReferenceToPrivateLikeType);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSemanticModelAction(context =>
        {
            INamedTypeSymbol privateAttributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName("privatelikeAttribute")!;

            SyntaxNode rootNode = context.SemanticModel.SyntaxTree.GetRoot(context.CancellationToken);

            foreach (SyntaxNode childNode in rootNode.DescendantNodes())
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                if (childNode is not IdentifierNameSyntax nameNode)
                {
                    continue;
                }

                TypeInfo typeInfo = context.SemanticModel.GetTypeInfo(nameNode, context.CancellationToken);

                if (typeInfo.Type is not INamedTypeSymbol typeSymbol)
                {
                    continue;
                }

                if (!typeSymbol.HasAttributeWithType(privateAttributeSymbol))
                {
                    continue;
                }

                if (nameNode.FirstAncestorOrSelf<TypeDeclarationSyntax>() is not { } parentNode)
                {
                    continue;
                }

                if (context.SemanticModel.GetDeclaredSymbol(parentNode, context.CancellationToken) is not { } parentSymbol)
                {
                    continue;
                }

                if (!SymbolEqualityComparer.Default.Equals(parentSymbol, typeSymbol) &&
                    !SymbolEqualityComparer.Default.Equals(parentSymbol, typeSymbol.ContainingType))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        InvalidReferenceToPrivateLikeType,
                        nameNode.GetLocation(),
                        typeSymbol,
                        parentSymbol));
                }
            }
        });
    }
}