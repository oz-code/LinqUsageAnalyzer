using System.Threading.Tasks;
using Octokit;

namespace LinqUsageAnalyzer.GitHub
{
    public class CredentialsStore : ICredentialStore
    {
        public Task<Credentials> GetCredentials()
        {
            return Task.Run(() => new Credentials("username", "password"));
        }
    }

}