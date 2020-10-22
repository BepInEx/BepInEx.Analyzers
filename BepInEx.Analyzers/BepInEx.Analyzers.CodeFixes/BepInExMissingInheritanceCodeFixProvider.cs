using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace BepInEx.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BepInExMissingInheritanceCodeFixProvider)), Shared]
    public class BepInExMissingInheritanceCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(BepInExMissingInheritanceAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            var codeAction = CodeAction.Create(CodeFixResources.BepInExMissingInheritanceCodeFixTitle, c => AddBaseListAsync(context.Document, declaration, c), nameof(CodeFixResources.BepInExMissingInheritanceCodeFixTitle));
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> AddBaseListAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            //Remove the trailing trivia from the class declaration
            var identifier = classDeclaration.Identifier;
            var classDeclarationTrimmed = classDeclaration.ReplaceToken(identifier, identifier.WithTrailingTrivia(SyntaxTriviaList.Empty));

            //Create a new BaseList
            var baseList = BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(IdentifierName("BaseUnityPlugin"))));

            //Produce the new class declaration
            var classDeclarationNew = classDeclarationTrimmed.WithBaseList(baseList);

            //Replace the old class declaration with the new class declaration
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(classDeclaration, classDeclarationNew);

            //Return document with transformed tree
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
