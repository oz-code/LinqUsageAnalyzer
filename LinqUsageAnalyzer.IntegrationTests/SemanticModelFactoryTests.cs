using System.Linq;
using NUnit.Framework;

namespace LinqUsageAnalyzer.IntegrationTests
{
    [TestFixture]
    public class SemanticModelFactoryTests
    {
        [Test]
        public void GetSematicModelAsync_PassValidSolutionWithTwoProjectsAndOneFileSharedBetweenProjects_DoNOtReturnDuplicateFiles()
        {
            var fileName = TestUtilities.ExtractResource("SolutionWithTwoProjectsSharingFiles.zip");

            var fileEngine = new FileEngine();
            var extractedFolder = fileEngine.Extract(fileName);

            var factory = new SemanticModelFactory();
            var result = factory.CreateSemanticModels(extractedFolder).ToArray();

            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}
