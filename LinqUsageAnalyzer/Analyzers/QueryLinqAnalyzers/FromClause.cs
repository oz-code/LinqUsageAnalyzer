using System;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LinqUsageAnalyzer.Analyzers.QueryLinqAnalyzers
{
    public class FromClause : IQueryClauseAnalyzer
    {
        public void AddToStatistics(SyntaxNode clause, RepositoryCounters result, Action<SyntaxToken, RepositoryCounters> saveStatistics)
        {
            var formClause = clause as FromClauseSyntax;
            if (formClause == null)
                return;

            saveStatistics(formClause.FromKeyword, result);
        }
    }
}