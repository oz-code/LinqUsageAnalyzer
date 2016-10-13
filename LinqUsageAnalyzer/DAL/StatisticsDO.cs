using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqUsageAnalyzer.DAL
{
    public class StatisticsDO
    {
        public StatisticsDO()
        {
            FluentLinqFound = new List<LinqFoundDO>();
            QueryLinqFound = new List<LinqFoundDO>();
        }

        public int LinesOfCode { get; set; }
        public int AnalyzedModels { get; set; }
        
        public double LinesOfCodePerLinq
        {
            get
            {
                double numberOfLinqQueries = FluentLinqFound.Count + QueryLinqFound.Count;
                if (numberOfLinqQueries == 0.0)
                {
                    return double.PositiveInfinity;
                }

                return LinesOfCode / numberOfLinqQueries;
            }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public int FluentLinqCalls => FluentLinqFound.Sum(found => found.Count);
        public int QueryLinqCalls => QueryLinqFound.Sum(found => found.Count);

        public string FluentFound
            => string.Join(" " + Environment.NewLine, FluentLinqFound.Where(found => found.Count > 0).Select(found => found.Name));

        public string QueryFound
            => string.Join(" " + Environment.NewLine, QueryLinqFound.Where(found => found.Count > 0).Select(found => found.Name));

        public List<LinqFoundDO>  FluentLinqFound { get; set; }
        public List<LinqFoundDO>  QueryLinqFound { get; set; }
    }
}