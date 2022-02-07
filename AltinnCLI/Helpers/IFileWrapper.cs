using System.IO;

namespace AltinnCLI.Helpers
{
    public interface IFileWrapper
    {
        bool SaveToFile(string filePath, string fileName, Stream stream);

        bool SaveToFile(string filePath, string fileName, string content);

        MemoryStream GetFile(string fullFileName);
    }
}
