using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Core
{
    public interface IFileWrapper
    {
        bool SaveToFile(string filePath, string fileName, Stream stream);

        MemoryStream GetFile(string fullFileName);
    }
}
