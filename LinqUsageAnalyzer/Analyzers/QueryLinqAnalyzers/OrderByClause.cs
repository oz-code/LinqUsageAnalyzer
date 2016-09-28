using System;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LinqUsageAnalyzer.Analyzers.QueryLinqAnalyzers
{
    public class OrderByClause : IQueryClauseAnalyzer
    {
        public void AddToStatistics(SyntaxNode clause, RepositoryCounters result, Action<SyntaxToken, RepositoryCounters> SaveStatistics)
        {
            var orderByClause = clause as OrderByClauseSyntax;
            if (orderByClause == null)
                return;

            foreach (var orderingSyntax in orderByClause.Orderings)
            {
                SaveStatistics(orderingSyntax.AscendingOrDescendingKeyword, result);
            }
        }
    }
}