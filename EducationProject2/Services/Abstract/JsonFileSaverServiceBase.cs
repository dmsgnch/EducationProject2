using System.Threading.Tasks;

namespace EducationProject2.Services.Abstract
{
    internal abstract class JsonFileSaverServiceBase : JsonSaveWorker
    {
        internal abstract Task SaveToFileAsync<T>(T objectToSave);
    }
}