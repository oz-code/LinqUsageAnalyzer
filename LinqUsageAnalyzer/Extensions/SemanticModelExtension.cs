using Microsoft.CodeAnalysis;

namespace LinqUsageAnalyzer.Extensions
{
    public static class SemanticModelExtension
    {
        public static int GetNumberOfLines(this SemanticModel semanticModel)
        {
            var codeText = semanticModel.SyntaxTree.ToString();

            int count = 1;
            int start = 0;
            while ((start = codeText.IndexOf('\n', start)) != -1)
            {
                count++;
                start++;
            }
            return count;
        }
    }
}
