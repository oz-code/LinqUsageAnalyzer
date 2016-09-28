using System;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LinqUsageAnalyzer.Analyzers.QueryLinqAnalyzers
{
    public class JoinClause : IQueryClauseAnalyzer
    {
        public void AddToStatistics(SyntaxNode clause, RepositoryCounters result, Action<SyntaxToken, RepositoryCounters> SaveStatistics)
        {
            var joinClause = clause as JoinClauseSyntax;
            if (joinClause == null)
                return;

            SaveStatistics(joinClause.JoinKeyword, result);

            if (joinClause.Into != null)
            {
                SaveStatistics(joinClause.Into.IntoKeyword, result);
            }

            SaveStatistics(joinClause.EqualsKeyword, result);
        }
    }
}