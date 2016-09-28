using LinqUsageAnalyzer.DAL;
using LinqUsageAnalyzer.GitHub;

namespace LinqUsageAnalyzer
{
    class Program
    {
        private const int Invalid = -1;
        private const int DefaultMaxProjects = 20;

        public static void Main(string[] args)
        {
            var maxProjects = ParseArgs(args);
            if (maxProjects == Invalid)
            {
                maxProjects = DefaultMaxProjects;
            }

            var analyzer = new AnalyzerManager(new GitHubEngine(), new FileEngine(), new InMemoryStatisticsRepository());

            analyzer.StartAsync(maxProjects).Wait();
        }

        private static int ParseArgs(string[] args)
        {
            if (args == null || args.Length <= 0)
                return Invalid;

            int newMaxProjects;
            if (!int.TryParse(args[0], out newMaxProjects))
                return Invalid;

            if (newMaxProjects > 0)
            {
                return newMaxProjects;
            }

            return Invalid;
        }
    }
}
