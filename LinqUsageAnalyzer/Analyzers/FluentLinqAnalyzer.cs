using System.Collections.Generic;
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
        private readonly SemanticModel _semantic;

        public FluentLinqAnalyzer(SemanticModel semantic)
        {
            _semantic = semantic;
            _root = semantic.SyntaxTree.GetRoot();

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
                SymbolInfo symbolInfo = _semantic.GetSymbolInfo(fluentExpressionSyntax);
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
            return _root.DescendantNodes().
                OfType<InvocationExpressionSyntax>();
        }
    }
}