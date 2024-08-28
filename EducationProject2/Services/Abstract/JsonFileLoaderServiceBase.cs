using System.Threading.Tasks;

namespace EducationProject2.Services.Abstract
{
    internal abstract class JsonFileLoaderServiceBase : JsonSaveWorker
    {
        internal abstract Task<T> GetFileDataOrNullAsync<T>() where T : class;
    }
}