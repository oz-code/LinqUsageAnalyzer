using System.IO;
using System.IO.Compression;
using Octokit;

namespace LinqUsageAnalyzer
{
    public class FileEngine
    {
        public string Extract(string filename)
        {
            var destinationDirectoryName = GetDestinationDirectoryName(filename);

            if (Directory.Exists(destinationDirectoryName))
            {
                Directory.Delete(destinationDirectoryName, true);
            }
            ZipFile.ExtractToDirectory(filename, destinationDirectoryName);

            return destinationDirectoryName;
        }

        public string GetDestinationDirectoryName(string filename)
        {
            var fileInfo = new FileInfo(filename);
            string currentFileName = fileInfo.FullName;
            string destinationDirectoryName = currentFileName.Remove(currentFileName.Length - fileInfo.Extension.Length) +
                                              "_extract";
            return destinationDirectoryName;
        }
    }
}