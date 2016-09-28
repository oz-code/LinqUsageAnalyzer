using System;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LinqUsageAnalyzer.Analyzers.QueryLinqAnalyzers
{
    public class GroupClause : IQueryClauseAnalyzer
    {
        public void AddToStatistics(SyntaxNode clause, RepositoryCounters result, Action<SyntaxToken, RepositoryCounters> SaveStatistics)
        {
            var groupByClause = clause as GroupClauseSyntax;
            if (groupByClause == null)
                return;

            SaveStatistics(groupByClause.GroupKeyword, result);
            SaveStatistics(groupByClause.ByKeyword, result);
        }
    }
}