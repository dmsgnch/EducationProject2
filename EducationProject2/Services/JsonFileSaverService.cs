using System;
using System.Threading.Tasks;
using Windows.Storage;
using EducationProject2.Services.Abstract;
using Newtonsoft.Json;

namespace EducationProject2.Services
{
    internal sealed class JsonFileSaverService : JsonFileSaverServiceBase
    {
        internal override async Task SaveToFileAsync<T>(T objectToSave)
        {
            StorageFile storageFile = await GetFileOrCreateAsync();
            
            string saveJson = JsonConvert.SerializeObject(objectToSave);
            await FileIO.WriteTextAsync(storageFile, saveJson);
        }

        private async Task<StorageFile> GetFileOrCreateAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            return await localFolder.CreateFileAsync(FilePath, CreationCollisionOption.ReplaceExisting);
        }
    }
}