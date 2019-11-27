using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltinnCLI.Core
{
    public class FileWrapper : IFileWrapper
    {
        private static ILogger _logger;

        public FileWrapper(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///  saves a file to disk
        /// </summary>
        /// <param name="filePath">the path excluded a base path for where the file shall be saved</param>
        /// <param name="fileName">filname to be used on the saved file</param>
        /// <param name="stream">the fiel content</param>
        public virtual bool SaveToFile(string filePath, string fileName, Stream stream)
        {
            string baseFolder = (ApplicationManager.ApplicationConfiguration.GetSection("StorageOutputFolder").Get<string>());
            string fileFolder = $@"{baseFolder}\{filePath}";

            // chekc if file folder exists, if not create it
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);

            }

            using (FileStream outputFile = new FileStream(Path.Combine(fileFolder, fileName), FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.CopyTo(outputFile);
            }

            return true;
        }

        /// <summary>
        /// Loads a file from disk and returns it as a memory stream. Error handling of existens of file is 
        ///  the clients responsibily.
        /// </summary>
        /// <param name="fullFileName">Fulle path including name of the file to load</param>
        /// <returns>memory stream with file content</returns>
        public virtual MemoryStream GetFile(string fullFileName)
        {
            FileStream fileStream = new FileStream(fullFileName, FileMode.Open);
            MemoryStream memoryStream = new MemoryStream(new byte[fileStream.Length]);
            fileStream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            fileStream.Close();

            return memoryStream;
        }
    }
}
