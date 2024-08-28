using System;
using System.Threading.Tasks;
using Windows.Storage;
using EducationProject2.Services.Abstract;
using Newtonsoft.Json;

namespace EducationProject2.Services
{
    internal sealed class JsonFileLoaderService : JsonFileLoaderServiceBase
    {
        internal override async Task<T> GetFileDataOrNullAsync<T>()
        {
            StorageFile storageFile = await GetFileOrNullAsync();
            if (storageFile is null) return null;
            
            string jsonData = await FileIO.ReadTextAsync(storageFile);
            return GetDeserializedFileData<T>(jsonData);
        }
        
        private async Task<StorageFile> GetFileOrNullAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            IStorageItem item = await localFolder.TryGetItemAsync(FilePath);

            if (item != null && item.IsOfType(StorageItemTypes.File))
            {
                return item as StorageFile;
            }

            return null;
        }
        
        private T GetDeserializedFileData<T>(string fileData)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(fileData);
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception($"Deserialize error {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error: {ex.Message}");
            }
        }
    }
}