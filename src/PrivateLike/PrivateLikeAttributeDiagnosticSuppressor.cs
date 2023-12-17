using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PrivateLike;

/// <summary>
/// A diagnostic analyzer for type conflicts for <c>[privatelike]</c> uses
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PrivateLikeAttributeDiagnosticSuppressor : DiagnosticSuppressor
{
    /// <summary>
    /// The diagnostic suppression for conflicts for <c>[privatelike]</c> uses
    /// </summary>
    public static readonly SuppressionDescriptor PrivateLikeAttributeAssemblyConflict = new(
        id: "PRIVSPR0001",
        suppressedDiagnosticId: "CS0436",
        justification: "It is expected to have multiple [privatelike] attributes, and the default choice from Roslyn is fine to use.");

    /// <inheritdoc/>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(PrivateLikeAttributeAssemblyConflict);

    /// <inheritdoc/>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            SyntaxNode? syntaxNode = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken).FindNode(diagnostic.Location.SourceSpan);

            if (syntaxNode is null)
            {
                continue;
            }

            SemanticModel semanticModel = context.GetSemanticModel(syntaxNode.SyntaxTree);
            TypeInfo typeInfo = semanticModel.GetTypeInfo(syntaxNode, context.CancellationToken);

            if (typeInfo.Type is INamedTypeSymbol { Name: "privatelikeAttribute" })
            {
                context.ReportSuppression(Suppression.Create(PrivateLikeAttributeAssemblyConflict, diagnostic));
            }
        }
    }
}