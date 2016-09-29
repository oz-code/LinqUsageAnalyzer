using System.IO;
using System.Net;
using System.Threading.Tasks;
using LinqUsageAnalyzer.Interfaces;
using Octokit;
using FileMode = System.IO.FileMode;

namespace LinqUsageAnalyzer.GitHub
{
    public class GitHubEngine : ICodeRepository
    {
        private readonly GitHubClient _gitHubClient;

        public GitHubEngine()
        {
            _gitHubClient = new GitHubClient(new ProductHeaderValue("TestGitHubAPI"), new CredentialsStore());
        }

        public async Task<SearchRepositoryResult> FindCSharpRepositoriesAsync(int fromPage = 0)
        {
            var searchRepositoriesRequest = new SearchRepositoriesRequest()
            {
                Language = Language.CSharp,
                SortField = RepoSearchSort.Stars,
                Order = SortDirection.Descending,
                PerPage = 10,
                Page = fromPage
            };

            return await _gitHubClient.Search.SearchRepo(searchRepositoriesRequest);
        }

        public async Task<Repository> GetRepositoryAsync(Repository repository)
        {
            return await _gitHubClient.Repository.Get(repository.Owner.Login, repository.Name);
        }

        public async Task<string> DownloadSourceRepositoryCodeAsync(Repository repository, string baseDirectory)
        {
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
            
            var fileName = Path.Combine(baseDirectory, repository.Name + ".zip");
            if (File.Exists(fileName))
            {// Avoid re-downloading
                return fileName;
            }

            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(repository.HtmlUrl + @"/archive/master" + ".zip", fileName);
            }

            return fileName;
        }
    }
}