using System.IO;

namespace AltinnCLI.Helpers
{
    public interface IFileWrapper
    {
        bool SaveToFile(string filePath, string fileName, Stream stream);

        MemoryStream GetFile(string fullFileName);
    }
}
