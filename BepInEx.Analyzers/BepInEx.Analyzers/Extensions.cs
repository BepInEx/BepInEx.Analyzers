﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BepInEx.Analyzers
{
    public static class Extensions
    {
        public static T GetRawSymbol<T>(this SemanticModel semanticModel, SyntaxNode syntax) where T : class, ISymbol
        {
            var symbol = semanticModel.GetSymbolInfo(syntax).Symbol;

            if(symbol == null)
                symbol = semanticModel.GetDeclaredSymbol(syntax);

            if(symbol == null)
                symbol = semanticModel.GetPreprocessingSymbolInfo(syntax).Symbol;

            if(symbol == null)
                return symbol as T;
            else
                return symbol.OriginalDefinition as T;
        }

        public static bool InheritsFrom(this ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken, string typeName)
        {
            var symbol = semanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);

            while(true)
            {
                if(symbol.ToString() == typeName)
                {
                    return true;
                }

                if(symbol.BaseType != null)
                {
                    symbol = symbol.BaseType;
                    continue;
                }

                break;
            }

            return false;
        }

        public static bool InheritsFrom(this INamedTypeSymbol symbol, string typeName)
        {
            while(true)
            {
                if(symbol.ToString() == typeName)
                {
                    return true;
                }

                if(symbol.BaseType != null)
                {
                    symbol = symbol.BaseType;
                    continue;
                }

                break;
            }

            return false;
        }

        public static bool HasAttribute(this MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken, string typeName)
        {
            foreach(var attributeList in memberDeclaration.AttributeLists)
                foreach(var attribute in attributeList.Attributes)
                {
                    var symbol = semanticModel.GetSymbolInfo(attribute, cancellationToken).Symbol;
                    if(symbol != null && symbol.ContainingType.ToString() == typeName)
                        return true;
                }

            return false;
        }

        public static bool HasAttribute(this ISymbol symbol, string typeName)
        {
            return symbol.GetAttributes().Any(x => x.AttributeClass.ToString() == typeName);
        }

        public static AttributeData GetAttribute(this ISymbol symbol, string typeName)
        {
            return symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass.ToString() == typeName);
        }

        public static bool IsStatic(this MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        }

        public static IEnumerable<INamedTypeSymbol> GetAllTypes(this INamespaceSymbol @namespace)
        {
            foreach(var type in @namespace.GetTypeMembers())
                foreach(var nestedType in GetNestedTypes(type))
                    yield return nestedType;

            foreach(var nestedNamespace in @namespace.GetNamespaceMembers())
                foreach(var type in GetAllTypes(nestedNamespace))
                    yield return type;
        }

        public static IEnumerable<INamedTypeSymbol> GetNestedTypes(this INamedTypeSymbol type)
        {
            yield return type;
            foreach(var nestedType in type.GetTypeMembers()
                .SelectMany(nestedType => GetNestedTypes(nestedType)))
                yield return nestedType;
        }

        public enum PropertyUsage
        {
            Get,
            Set,
            GetAndSet
        }

        // https://github.com/dotnet/roslyn/issues/15527
        // Allow to figure how the property was used,
        // Through its getter, or setter, or both.
        //
        // ++/--    pre/postfix increments            get/set
        // =        lhs of simple assignments         set
        // +=, -=   lhs of other assigments           get/set
        // x.y      rhs, of compound member access    recurr up
        //
        // any other use is just a get
        //
        // TOOD: ref parameters?
        //
        public static PropertyUsage GetPropertyUsage(this SyntaxNode node)
        {
            var kind = node.Parent.Kind();

            if (kind == SyntaxKind.PostIncrementExpression ||
                kind == SyntaxKind.PostDecrementExpression ||
                kind == SyntaxKind.PreIncrementExpression ||
                kind == SyntaxKind.PreDecrementExpression)
            {
                return PropertyUsage.GetAndSet;
            }
            else if (node.Parent is AssignmentExpressionSyntax)
            {
                var assignment = (AssignmentExpressionSyntax)(node.Parent);

                if (assignment.Left == node)
                {
                    return kind == SyntaxKind.SimpleAssignmentExpression
                        ? PropertyUsage.Set
                        : PropertyUsage.GetAndSet;
                }
            }
            else if (node.Parent is MemberAccessExpressionSyntax)
            {
                var m = (MemberAccessExpressionSyntax)(node.Parent);

                if (m.Name == node)
                {
                    return node.Parent.GetPropertyUsage();
                }
            }

            return PropertyUsage.Get;
        }
    }
}
