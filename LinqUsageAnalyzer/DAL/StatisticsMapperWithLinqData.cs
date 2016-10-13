using LinqUsageAnalyzer.Interfaces;

namespace LinqUsageAnalyzer.DAL
{
    class StatisticsMapperWithLinqData : StatisticsMapper
    {
        public override StatisticsDO CreateDataObject(RepositoryStatistics statistics)
        {
            var result = base.CreateDataObject(statistics);

            foreach (var linqUsed in statistics.Counters.LinqOperatorUsed)
            {
                if (linqUsed.Key.LinqKind == LinqKind.Fluent)
                {
                    result.FluentLinqFound.Add(new LinqFoundDO {Name = linqUsed.Key.Name, Count = linqUsed.Value});
                }
                else
                {
                    result.QueryLinqFound.Add(new LinqFoundDO { Name = linqUsed.Key.Name, Count = linqUsed.Value });
                }
            }

            return result;
        }
    }
}