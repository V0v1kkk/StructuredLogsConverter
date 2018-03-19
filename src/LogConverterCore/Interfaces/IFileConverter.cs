namespace LogConverterCore.Interfaces
{
    public interface IFileConverter
    {
        void ConvertFile(string filePath, bool deleteOrigin);
    }
}