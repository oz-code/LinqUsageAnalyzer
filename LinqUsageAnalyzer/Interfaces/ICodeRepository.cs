using System.Threading.Tasks;
using Octokit;

namespace LinqUsageAnalyzer.Interfaces
{
    public interface ICodeRepository
    {
        Task<SearchRepositoryResult> FindCSharpRepositoriesAsync(int fromPage = 0);
        Task<Repository> GetRepositoryAsync(Repository repository);
        Task<string> DownloadSourceRepositoryCodeAsync(Repository repository, string baseDirectory);
    }
}