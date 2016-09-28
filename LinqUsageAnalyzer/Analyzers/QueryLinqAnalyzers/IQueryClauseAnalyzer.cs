using System;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;

namespace LinqUsageAnalyzer.Analyzers.QueryLinqAnalyzers
{
    public interface IQueryClauseAnalyzer
    {
        void AddToStatistics(SyntaxNode clause, RepositoryCounters result, Action<SyntaxToken, RepositoryCounters> SaveStatistics);
    }
}