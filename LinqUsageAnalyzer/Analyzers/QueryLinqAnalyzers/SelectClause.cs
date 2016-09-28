using System;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LinqUsageAnalyzer.Analyzers.QueryLinqAnalyzers
{
    public class SelectClause : IQueryClauseAnalyzer
    {
        public void AddToStatistics(SyntaxNode clause, RepositoryCounters result, Action<SyntaxToken, RepositoryCounters> SaveStatistics)
        {
            var selectByClause = clause as SelectClauseSyntax;
            if (selectByClause == null)
                return;

            SaveStatistics(selectByClause.SelectKeyword, result);
        }
    }
}