using LinqUsageAnalyzer.Analyzers;
using LinqUsageAnalyzer.Extensions;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using NLog;

namespace LinqUsageAnalyzer
{
    public class SemanticModelAnalyzer
    {
        private readonly ILogger _log;

        public SemanticModelAnalyzer()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public void Analyze(SemanticModel semanticModel, RepositoryStatistics statistics)
        {
            statistics.LinesOfCode = semanticModel.GetNumberOfLines();

            AnalizeLinq(new QueryLinqAnalyzer(semanticModel), statistics);
            AnalizeLinq(new FluentLinqAnalyzer(semanticModel), statistics);
        }

        private void AnalizeLinq(IFileAnalyzer analyzer, RepositoryStatistics result)
        {
            if (analyzer.HasRelevantLinqQueries())
            {
                var counters = analyzer.Analyze();

                _log.Log(LogLevel.Trace, "-- Found {0} LINQ Operators --", counters.LinqOperatorUsed.Count);

                result.Counters.Append(counters);
            }
        }
    }
}