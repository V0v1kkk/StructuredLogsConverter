using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogConverterCore.Interfaces
{
    public interface IComponentsExtractor
    {
        Task<IList<string>> ExtractComponentsList(string filePath);
    }
}