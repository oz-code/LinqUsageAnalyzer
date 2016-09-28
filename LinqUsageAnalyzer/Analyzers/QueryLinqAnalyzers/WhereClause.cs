using System;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LinqUsageAnalyzer.Analyzers.QueryLinqAnalyzers
{
    public class WhereClause : IQueryClauseAnalyzer
    {
        public void AddToStatistics(SyntaxNode clause, RepositoryCounters result, Action<SyntaxToken, RepositoryCounters> SaveStatistics)
        {
            var whereClause = clause as WhereClauseSyntax;
            if (whereClause == null)
                return;

            SaveStatistics(whereClause.WhereKeyword, result);
        }
    }
}