using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace BepInEx.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BepInExMissingAttributeCodeFixProvider)), Shared]
    public class BepInExMissingAttributeCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(BepInExMissingAttributeAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            var codeAction = CodeAction.Create(CodeFixResources.BepInExMissingAttributeCodeFixTitle, c => AddBepInPluginAttributeAsync(context.Document, declaration, c), nameof(CodeFixResources.BepInExMissingAttributeCodeFixTitle));
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> AddBepInPluginAttributeAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var attributes = classDeclaration.AttributeLists.Add(
                AttributeList(
                    SingletonSeparatedList(
                        Attribute(IdentifierName("BepInPlugin"))
                            .WithArgumentList(
                                AttributeArgumentList(
                                    SeparatedList<AttributeArgumentSyntax>(
                                        new SyntaxNodeOrToken[]
                                        {
                                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("GUID"))),
                                            Token(SyntaxKind.CommaToken),
                                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Name"))),
                                            Token(SyntaxKind.CommaToken),
                                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("1.0")))
                                        }))))));

            return document.WithSyntaxRoot(root.ReplaceNode(classDeclaration, classDeclaration.WithAttributeLists(attributes)));
        }
    }
}
