namespace LinqUsageAnalyzer.Interfaces
{
    public interface IFileAnalyzer
    {
        bool HasRelevantLinqQueries();
        RepositoryCounters Analyze();
    }
}