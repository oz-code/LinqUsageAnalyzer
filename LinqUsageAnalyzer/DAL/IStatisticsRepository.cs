using LinqUsageAnalyzer.Interfaces;

namespace LinqUsageAnalyzer.DAL
{
    public interface IStatisticsRepository
    {
        void Save(RepositoryStatistics statistics);
    }
}