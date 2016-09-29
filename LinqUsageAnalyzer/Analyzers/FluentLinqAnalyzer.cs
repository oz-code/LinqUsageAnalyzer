using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LinqUsageAnalyzer.Analyzers
{
    public class FluentLinqAnalyzer : IFileAnalyzer
    {
        private const string LinqDeclaration = "System.Linq.Enumerable";

        private readonly SyntaxNode _root;
        private readonly SemanticModel _semanticModel;
        private static ImmutableHashSet<string> _LINQOperatorNames;

        public FluentLinqAnalyzer(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            _root = semanticModel.SyntaxTree.GetRoot();

        }
        public bool HasRelevantLinqQueries()
        {
            return GetFluentLinqNodes().Any();
        }

        public RepositoryCounters Analyze()
        {
            var linqNodes = GetFluentLinqNodes().ToList();
         
            var result = new RepositoryCounters();
            foreach (InvocationExpressionSyntax fluentExpressionSyntax in linqNodes)
            {
                SymbolInfo symbolInfo = _semanticModel.GetSymbolInfo(fluentExpressionSyntax);
                IMethodSymbol symbol = symbolInfo.Symbol as IMethodSymbol;

                if (symbol != null && symbol.ConstructedFrom.ContainingType.ToString() == LinqDeclaration)
                {
                    result.Add(LinqKind.Fluent, symbol.Name);
                }
            }

            return result;
        }

        private IEnumerable<InvocationExpressionSyntax> GetFluentLinqNodes()
        {
            return _root
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(i => GetLINQOperatorNames().Contains(GetInvokedMethodName(i)));
        }

        private static string GetInvokedMethodName(InvocationExpressionSyntax i)
        {

            var memberAccess = i.Expression as MemberAccessExpressionSyntax;
            return memberAccess?.Name?.ToString() ?? string.Empty; 
        }

        private ImmutableHashSet<string> GetLINQOperatorNames()
        {
            return _LINQOperatorNames ?? (_LINQOperatorNames = _semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable").GetMembers().Select(m => m.Name).ToImmutableHashSet());
        }
    }
}