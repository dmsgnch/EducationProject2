using System.Collections.Generic;
using System.Threading.Tasks;
using EducationProject2.Models.Abstract;

namespace EducationProject2.Services.Abstract
{
    internal abstract class StorageServiceBase<T> where T : IMongoDbObject
    {
        //Combined functionality, because saving and loading are strongly linked
        internal abstract Task SaveAsync(List<T> objectToSave);
        internal abstract Task<List<T>> LoadAsync();
    }
}