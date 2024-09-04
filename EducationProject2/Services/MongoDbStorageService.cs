using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EducationProject2.Models.Abstract;
using EducationProject2.Services.Abstract;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EducationProject2.Services
{
    internal class MongoDbStorageService<T> : StorageServiceBase<T> where T : class, IMongoDbObject
    {
        private IMongoCollection<T> PersonCollection { get; }

        public MongoDbStorageService(string collectionName)
        {
            var client =
                new MongoClient("mongodb://localhost:27017/?connectTimeoutMS=1000&serverSelectionTimeoutMS=1000");
            var database = client.GetDatabase("EducationProject2Db");
            PersonCollection = database.GetCollection<T>(collectionName);
        }

        internal override async Task SaveAsync(List<T> objectToSave)
        {
            var existingElements = await GetCollectionDataAsync();

            await DeleteNonexistentElementsInCollectionAsync(existingElements, objectToSave);

            await AddOrUpdateElementsInCollectionAsync(objectToSave);
        }

        private async Task<List<T>> GetCollectionDataAsync()
        {
            try
            {
                return await PersonCollection.Find(FilterDefinition<T>.Empty).ToListAsync();
            }
            catch (TimeoutException ex)
            {
                throw new Exception(
                    $"The database connection time has expired!\n" +
                    $"Database is unavailable, please try again later or select another storage method!\n" +
                    $"All data will be saved locally!");
            }
            catch (MongoConnectionException ex)
            {
                throw new Exception($"MongoDB connection error!");
            }
            catch (Exception ex)
            {
                throw new Exception($"MongoDB unexpected error occurred!");
            }
        }

        private async Task DeleteNonexistentElementsInCollectionAsync(List<T> existingElements, List<T> newElements)
        {
            var elementsIdToDelete =
                existingElements.Where(obj => !newElements.Any(anotherObj => anotherObj.Id == obj.Id))
                    .Select(p => p.Id)
                    .ToList();
            if (elementsIdToDelete.Any())
            {
                await DeleteAllElementsByIdListAsync(elementsIdToDelete);
            }
        }

        private async Task DeleteAllElementsByIdListAsync(List<ObjectId> personsIdToDelete)
        {
            try
            {
                var deleteFilter = Builders<T>.Filter.In(p => p.Id, personsIdToDelete);
                await PersonCollection.DeleteManyAsync(deleteFilter);
            }
            catch (TimeoutException ex)
            {
                throw new Exception(
                    $"The database connection time has expired!\n" +
                    $"Database is unavailable, please try again later or select another storage method!\n" +
                    $"All data will be saved locally!");
            }
            catch (MongoConnectionException ex)
            {
                throw new Exception($"MongoDB connection error!");
            }
            catch (Exception ex)
            {
                throw new Exception($"MongoDB unexpected error occurred!");
            }
        }

        private async Task AddOrUpdateElementsInCollectionAsync(List<T> elements)
        {
            foreach (var element in elements)
            {
                if (element.Id == ObjectId.Empty)
                {
                    await PersonCollection.InsertOneAsync(element);
                }
                else
                {
                    await ReplaceElementAsync(element);
                }
            }
        }

        private async Task ReplaceElementAsync(T element)
        {
            try
            {
                var replaceFilter = Builders<T>.Filter.Eq(p => p.Id, element.Id);
                await PersonCollection.ReplaceOneAsync(replaceFilter, element,
                    new ReplaceOptions { IsUpsert = true });
            }
            catch (TimeoutException ex)
            {
                throw new Exception(
                    $"The database connection time has expired!\n" +
                    $"Database is unavailable, please try again later or select another storage method!\n" +
                    $"All data will be saved locally!");
            }
            catch (MongoConnectionException ex)
            {
                throw new Exception($"MongoDB connection error!");
            }
            catch (Exception ex)
            {
                throw new Exception($"MongoDB unexpected error occurred!");
            }
        }

        internal override async Task<List<T>> LoadAsync()
        {
            return await GetCollectionDataAsync();
        }
    }
}