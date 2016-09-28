using System;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LinqUsageAnalyzer.Analyzers.QueryLinqAnalyzers
{
    public class LetClause : IQueryClauseAnalyzer
    {
        public void AddToStatistics(SyntaxNode clause, RepositoryCounters result, Action<SyntaxToken, RepositoryCounters> SaveStatistics)
        {
            var letClause = clause as LetClauseSyntax;
            if (letClause == null)
                return;

            SaveStatistics(letClause.LetKeyword, result);
        }
    }
}