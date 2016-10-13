using System;
using System.Collections.Generic;
using Octokit;

namespace LinqUsageAnalyzer.Interfaces
{
    public class RepositoryCounters
    {
        public Dictionary<LinqOperator, int> LinqOperatorUsed { get; set; } = new Dictionary<LinqOperator, int>();

        public void Add(LinqKind linqKind, string symbolName)
        {
            var linqOp = new LinqOperator(linqKind, symbolName);

            if (!LinqOperatorUsed.ContainsKey(linqOp))
            {
                LinqOperatorUsed.Add(linqOp, 0);
            }

            LinqOperatorUsed[linqOp]++;
        }

        public void Append(RepositoryCounters counters)
        {
            foreach (var linqOpsData in counters.LinqOperatorUsed)
            {
                var linqOp = linqOpsData.Key;

                if (!LinqOperatorUsed.ContainsKey(linqOp))
                {
                    LinqOperatorUsed.Add(linqOp, 0);
                }

                LinqOperatorUsed[linqOp] += linqOpsData.Value;
            }
        }
    }
    public class RepositoryStatistics
    {
        public RepositoryStatistics(Repository repository)
        {
            Id = repository.Id;
            Name = repository.Name;
            FullName = repository.FullName;
            Url = repository.Url;
            HtmlUrl = repository.HtmlUrl;
            CreatedAt = repository.CreatedAt;
            UpdatedAt = repository.UpdatedAt;
            LinesOfCode = 0;
            AnalyzedModels = 0;
            Counters = new RepositoryCounters();
        }

        public int LinesOfCode { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public RepositoryCounters Counters { get; }
        public int AnalyzedModels { get; set; }
    }
}