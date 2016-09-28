using System.IO;
using System.Threading.Tasks;
using LinqUsageAnalyzer.GitHub;
using NUnit.Framework;

namespace LinqUsageAnalyzer.IntegrationTests
{
    [TestFixture, Explicit]
    public class GitHubManagerTests
    {
        [Test]
        public async Task FindCSharpRepositoriesAsync_ReturnAllCSharpRepositories()
        {
            var githubManager = new GitHubEngine();

            var result = await githubManager.FindCSharpRepositoriesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Items.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task DownloadSourceRepositoryCodeAsync_SaveRepositoryToFile()
        {
            var githubManager = new GitHubEngine();

            var result = await githubManager.FindCSharpRepositoriesAsync();
            var currentPath = Path.GetTempPath();
            
            var fileName = await githubManager.DownloadSourceRepositoryCodeAsync(result.Items[0], currentPath);

            Assert.That(File.Exists(fileName), Is.True);
        }
    }
}
