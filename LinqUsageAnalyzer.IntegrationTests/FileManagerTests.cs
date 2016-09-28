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

        private string ExtractResource(string fileName)
        {
            var tragetFile = Path.Combine(Path.GetTempPath(), fileName);

            Assembly a = Assembly.GetExecutingAssembly();

            using (Stream resourceStream = a.GetManifestResourceStream(@"LinqUsageAnalyzer.IntegrationTests.TestFiles." + fileName))
            using(var fileStream =  File.Open(tragetFile, FileMode.Create))
            {
                for (int i = 0; i < resourceStream.Length; i++)
                {
                    fileStream.WriteByte((byte)resourceStream.ReadByte());
                }
                fileStream.Close();
            }

            return tragetFile;
        }

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

            var fileName = ExtractResource(DemoProjectZip);

            var resultFolder = fileManager.Extract(fileName);

            Assert.That(resultFolder, Is.Not.Null.And.Not.Empty);
            Assert.That(Directory.Exists(resultFolder), Is.True);
        }

        [Test]
        public async Task Extract_ProvideValidRepositoryExtractTwice_ExtractAndReturnBaseFolder()
        {
            var fileManager = new FileEngine();

            var fileName = ExtractResource(DemoProjectZip);

            var resultFolder = fileManager.Extract(fileName);
            resultFolder = fileManager.Extract(fileName);

            Assert.That(resultFolder, Is.Not.Null.And.Not.Empty);
            Assert.That(Directory.Exists(resultFolder), Is.True);
        }
    }
}