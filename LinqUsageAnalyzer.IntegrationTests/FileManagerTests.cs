using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using FileMode = System.IO.FileMode;

namespace LinqUsageAnalyzer.IntegrationTests
{
    [TestFixture]
    public class FileManagerTests
    {
        private const string DemoProjectZip = "demo_project.zip";

        [SetUp]
        public void CleanFolder()
        {
            var fileManager = new FileEngine();
            var targetDirectoryName = fileManager.GetDestinationDirectoryName("demo_project.zip");
            if (Directory.Exists(targetDirectoryName))
            {
                Directory.Delete(targetDirectoryName, true);
            }
        }

        [Test]
        public async Task Extract_ProvideValidRepository_ExtractAndReturnBaseFolder()
        {
            var fileManager = new FileEngine();

            var fileName = TestUtilities.ExtractResource(DemoProjectZip);

            var resultFolder = fileManager.Extract(fileName);

            Assert.That(resultFolder, Is.Not.Null.And.Not.Empty);
            Assert.That(Directory.Exists(resultFolder), Is.True);
        }

        [Test]
        public async Task Extract_ProvideValidRepositoryExtractTwice_ExtractAndReturnBaseFolder()
        {
            var fileManager = new FileEngine();

            var fileName = TestUtilities.ExtractResource(DemoProjectZip);

            var resultFolder = fileManager.Extract(fileName);
            resultFolder = fileManager.Extract(fileName);

            Assert.That(resultFolder, Is.Not.Null.And.Not.Empty);
            Assert.That(Directory.Exists(resultFolder), Is.True);
        }
    }
}