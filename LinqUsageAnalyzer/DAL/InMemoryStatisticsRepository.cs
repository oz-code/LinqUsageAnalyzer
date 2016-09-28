using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinqUsageAnalyzer.Interfaces;

namespace LinqUsageAnalyzer.DAL
{

    class InMemoryStatisticsRepository : IStatisticsRepository
    {
        public List<StatisticsDO> SavedData { get; set; } = new List<StatisticsDO>();
        private readonly HashSet<LinqOperator> _linqOperatorsUsed = new HashSet<LinqOperator>();

        public InMemoryStatisticsRepository()
        {
            Mapper.Initialize(cfg =>
                cfg.CreateMap<RepositoryStatistics, StatisticsDO>()
                    .ForMember(x => x.QueryLinqFound, opt => opt.Ignore())
                    .ForMember(x => x.FluentLinqFound, opt => opt.Ignore()));

        }

        public void Save(RepositoryStatistics statistics)
        {
            UpdateExistingQueries(statistics);

            var data = CreateDataObject(statistics);

            SavedData.Add(data);
        }

        private StatisticsDO CreateDataObject(RepositoryStatistics statistics)
        {
            var data = Mapper.Map<StatisticsDO>(statistics);

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