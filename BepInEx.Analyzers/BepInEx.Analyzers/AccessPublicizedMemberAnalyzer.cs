using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BepInEx.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AccessPublicizedMemberAnalyzer : DiagnosticAnalyzer
    {
        private enum AccessModifier
        {
            // Technically not used since public members don't get PublicizedMemberAttribute
            Public = 0,
            Private = 1,
            Protected = 2,
            Internal = 3,
            ProtectedInternal = 4,
            PrivateProtected = 5
        }

        private static bool IsPrivate(AccessModifier accessibility) =>
            accessibility == AccessModifier.Private ||
            accessibility == AccessModifier.PrivateProtected ||
            accessibility == AccessModifier.Internal;

        private static bool IsProtected(AccessModifier accessibility) =>
            accessibility == AccessModifier.Protected ||
            accessibility == AccessModifier.ProtectedInternal;

        public const string DiagnosticId = "Publicizer001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AccessPublicizedMemberAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AccessPublicizedMemberAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AccessPublicizedMemberAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Member Access";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        // https://github.com/BepInEx/UnityDataMiner/blob/master/UnityDataMiner/AssemblyStripper.cs
        private const string PublicizedAttributeName = "System.Runtime.CompilerServices.PublicizedAttribute";

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSimpleMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess.Name, context.CancellationToken).Symbol;
            
            if (symbol == null)
                return;

            if (symbol is IPropertySymbol propertySymbol)
            {
                var propertyUsage = context.Node.GetPropertyUsage();
                if (propertyUsage == Extensions.PropertyUsage.Get || propertyUsage == Extensions.PropertyUsage.GetAndSet)
                {
                    var getMethodPublicizedAttribute = propertySymbol.GetMethod.GetAttribute(PublicizedAttributeName);
                    Check(getMethodPublicizedAttribute, symbol, context, memberAccess);
                }
                else
                {
                    var setMethodPublicizedAttribute = propertySymbol.SetMethod.GetAttribute(PublicizedAttributeName);
                    Check(setMethodPublicizedAttribute, symbol, context, memberAccess);
                }
            }
            else
            {
                var publicizedAttribute = symbol.GetAttribute(PublicizedAttributeName);
                Check(publicizedAttribute, symbol, context, memberAccess);
            }
        }

        private void Check(AttributeData attribute, ISymbol symbol, SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (attribute != null)
            {
                var accessibility = GetAccessModifier(attribute);
                Check(accessibility, symbol, context, memberAccess);
            }
        }

        private static AccessModifier GetAccessModifier(AttributeData publicizedAttribute)
        {
            return (AccessModifier)publicizedAttribute.ConstructorArguments[0].Value;
        }

        private static void Check(AccessModifier accessibility, ISymbol symbol, SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (IsPrivate(accessibility) ||
               (IsProtected(accessibility) && IsUsedOutsideClassDefinition(symbol.ContainingType, context)))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, memberAccess.Name.GetLocation(), memberAccess.Name.Identifier));
            }
        }

        private static bool IsUsedOutsideClassDefinition(INamedTypeSymbol containingType, SyntaxNodeAnalysisContext context)
        {
            var contextBaseType = context.ContainingSymbol.ContainingType?.BaseType;
            while (contextBaseType != null)
            {
                if (SymbolEqualityComparer.Default.Equals(containingType, contextBaseType))
                {
                    return false;
                }

                contextBaseType = contextBaseType.BaseType;
            }

            return true;
        }
    }
}
