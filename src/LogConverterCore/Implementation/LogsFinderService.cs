using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogConverterCore.Interfaces;
using Serilog;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace LogConverterCore.Implementation
{
    public class LogsFinderService : ILogFinder
    {
        private readonly ILogger _systemLogger;

        public LogsFinderService(ILogger systemLogger)
        {
            _systemLogger = systemLogger;
        }

        public IEnumerable<string> FindLogFilesInFolder(string folderPath, bool oneLevel = false)
        {
            if(string.IsNullOrEmpty(folderPath)) throw new 
                NullReferenceException($"{nameof(folderPath)} is null or empty.");
            if(!Directory.Exists(folderPath)) throw new 
                DirectoryNotFoundException($"{nameof(folderPath)} not exist" );


            return Directory.EnumerateFiles(folderPath, "*.json",
                oneLevel ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        public IEnumerable<string> FindAndUnpackLogArchivesInFolder(string folderPath, bool oneLevel = false)
        {
            if (string.IsNullOrEmpty(folderPath)) throw new
                NullReferenceException($"{nameof(folderPath)} is null or empty.");
            if (!Directory.Exists(folderPath)) throw new
                DirectoryNotFoundException($"{nameof(folderPath)} not exist");

            var archivesPaths = Directory.EnumerateFiles(folderPath, "*.zip",
                oneLevel ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);

            if(!archivesPaths.Any()) return new List<string>();

            var extractedFilesPaths = new List<string>();
            var tempFolder = GetTemporaryDirectory();

            foreach (var archivePath in archivesPaths)
            {
                var archiveName = Path.GetFileNameWithoutExtension(archivePath);

                try
                {
                    using (ZipArchive zipArchive = ZipArchive.Open(archivePath))
                    {
                        foreach (ZipArchiveEntry entry in zipArchive.Entries)
                        {
                            if (!entry.Key.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) continue;

                            string destinationFileName = null;
                            try
                            {
                                destinationFileName = Path.Combine(tempFolder, $"{archiveName}_{entry.Key}");
                                entry.WriteToFile(destinationFileName);

                                extractedFilesPaths.Add(destinationFileName);
                            }
                            catch (Exception exception)
                            {
                                _systemLogger.Error(exception, "Error on extracting: {EntryFullName}. Extracting to: {OutputPath}", 
                                    entry.Key, destinationFileName);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    _systemLogger.Error(exception, "Error on extracting from archive: {ArchivePath}", archivePath);
                } 
            }

            return extractedFilesPaths;
        }

        private static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }


    }
}
