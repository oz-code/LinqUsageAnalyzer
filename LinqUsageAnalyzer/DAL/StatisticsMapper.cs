using AutoMapper;
using LinqUsageAnalyzer.Interfaces;

namespace LinqUsageAnalyzer.DAL
{
    class StatisticsMapper
    {
        public StatisticsMapper()
        {
            Mapper.Initialize(cfg =>
                cfg.CreateMap<RepositoryStatistics, StatisticsDO>()
                    .ForMember(x => x.QueryLinqFound, opt => opt.Ignore())
                    .ForMember(x => x.FluentLinqFound, opt => opt.Ignore()));

        }

        public virtual StatisticsDO CreateDataObject(RepositoryStatistics statistics)
        {
            return Mapper.Map<StatisticsDO>(statistics);
        }
    }
}