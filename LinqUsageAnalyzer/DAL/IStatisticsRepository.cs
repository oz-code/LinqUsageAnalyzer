using System.Threading.Tasks;
using LinqUsageAnalyzer.Interfaces;

namespace LinqUsageAnalyzer.DAL
{
    public interface IStatisticsRepository
    {
        Task SaveAsync(RepositoryStatistics statistics);
        bool RepositoryExist(string repositoryName);
    }
}