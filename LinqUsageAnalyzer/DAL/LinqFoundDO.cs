namespace LinqUsageAnalyzer.DAL
{
    public class LinqFoundDO
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Count)}: {Count}";
        }
    }
}