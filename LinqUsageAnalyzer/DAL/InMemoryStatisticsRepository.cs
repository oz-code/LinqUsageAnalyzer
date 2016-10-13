using System.Collections.Generic;
using System.Threading.Tasks;
using LinqUsageAnalyzer.Interfaces;

namespace LinqUsageAnalyzer.DAL
{
    class InMemoryStatisticsRepository : IStatisticsRepository
    {
        public List<StatisticsDO> SavedData { get; set; } = new List<StatisticsDO>();
        private readonly HashSet<LinqOperator> _linqOperatorsUsed = new HashSet<LinqOperator>();
        private readonly StatisticsMapper _mapper;

        public InMemoryStatisticsRepository()
        {
            _mapper = new StatisticsMapper();

        }

        public Task SaveAsync(RepositoryStatistics statistics)
        {
            UpdateExistingQueries(statistics);

            var data = CreateDataObject(statistics);

            SavedData.Add(data);

            return Task.FromResult(true);
        }

        public bool RepositoryExist(string repositoryName)
        {
            return false;
        }

        private StatisticsDO CreateDataObject(RepositoryStatistics statistics)
        {
            var data = _mapper.CreateDataObject(statistics);

            foreach (var linqOperator in _linqOperatorsUsed)
            {
                var linqFoundDo = new LinqFoundDO {Name = linqOperator.Name, Count = 0};

                if (statistics.Counters.LinqOperatorUsed.ContainsKey(linqOperator))
                {
                    linqFoundDo.Count = statistics.Counters.LinqOperatorUsed[linqOperator];
                }

                if (linqOperator.LinqKind == LinqKind.Fluent)
                {
                    data.FluentLinqFound.Add(linqFoundDo);
                }
                else
                {
                    data.QueryLinqFound.Add(linqFoundDo);
                }
            }
           
            return data;
        }

        private void UpdateExistingQueries(RepositoryStatistics statistics)
        {
            foreach (var linqUsed in statistics.Counters.LinqOperatorUsed)
            {
                if (!_linqOperatorsUsed.Contains(linqUsed.Key))
                {
                    _linqOperatorsUsed.Add(linqUsed.Key);
                    foreach (var statisticsDo in SavedData)
                    {
                        if (linqUsed.Key.LinqKind == LinqKind.Fluent)
                        {
                            statisticsDo.FluentLinqFound.Add(new LinqFoundDO {Count = 0, Name = linqUsed.Key.Name});
                        }
                        else
                        {
                            statisticsDo.QueryLinqFound.Add(new LinqFoundDO {Count = 0, Name = linqUsed.Key.Name});
                        }
                    }
                }
            }
        }
    }
}