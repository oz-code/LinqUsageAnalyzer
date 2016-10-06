using System.IO;
using System.Reflection;

namespace LinqUsageAnalyzer.IntegrationTests
{
    public static class TestUtilities
    {
        public static string ExtractResource(string fileName)
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
    }
}