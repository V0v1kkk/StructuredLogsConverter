using System.Collections.Generic;

namespace LogConverterCore.Interfaces
{
    public interface ILogFinder
    {
        IEnumerable<string> FindLogFilesInFolder(string folderPath, bool oneLevel = false);
        IEnumerable<string> FindAndUnpackLogArchivesInFolder(string folderPath, bool oneLevel = false);
    }
}