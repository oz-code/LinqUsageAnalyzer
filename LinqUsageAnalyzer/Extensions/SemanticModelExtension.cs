using Microsoft.CodeAnalysis;

namespace LinqUsageAnalyzer.Extensions
{
    public static class SemanticModelExtension
    {
        public static int GetNumberOfLines(this SemanticModel semanticModel)
        {
            return semanticModel.SyntaxTree.GetText().Lines.Count;
        }
    }
}
