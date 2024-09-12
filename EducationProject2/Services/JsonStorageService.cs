using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using EducationProject2.Models.Abstract;
using EducationProject2.Services.Abstract;
using Newtonsoft.Json;

namespace EducationProject2.Services
{
    internal class JsonStorageService<T> : StorageServiceBase<T> where T : IMongoDbObject
    {
        private const string JsonFilePath = "save.json";

        internal override async Task SaveAsync(List<T> objectToSave)
        {
            StorageFile storageFile = await GetFileOrCreateAsync();

            string saveJson = JsonConvert.SerializeObject(objectToSave);
            await FileIO.WriteTextAsync(storageFile, saveJson);
        }

        #region Saving inner functionality

        private async Task<StorageFile> GetFileOrCreateAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            return await localFolder.CreateFileAsync(JsonFilePath, CreationCollisionOption.ReplaceExisting);
        }

        #endregion

        internal override async Task<List<T>> LoadAsync()
        {
            List<T> loadedCollection = new List<T>();

            StorageFile storageFile = await GetFileOrNullAsync();
            if (storageFile != null)
            {
                string jsonData = await FileIO.ReadTextAsync(storageFile);
                if (!string.IsNullOrWhiteSpace(jsonData))
                {
                    loadedCollection = GetDeserializedFileData<List<T>>(jsonData);
                }
            }

            return loadedCollection;
        }

        #region Loading inner functionality

        private async Task<StorageFile> GetFileOrNullAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            IStorageItem item = await localFolder.TryGetItemAsync(JsonFilePath);

            return item != null && item.IsOfType(StorageItemTypes.File) ? (StorageFile)item : null;
        }

        private TCollection GetDeserializedFileData<TCollection>(string fileData)
        {
            try
            {
                return JsonConvert.DeserializeObject<TCollection>(fileData);
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Json deserialize error: {ex.Message}");
                throw new Exception($"Json deserialize error!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occured during json deserialization: {ex.Message}");
                throw new Exception($"Unexpected error occured during json deserialization!");
            }
        }

        #endregion
    }
}