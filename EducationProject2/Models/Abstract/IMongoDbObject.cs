using MongoDB.Bson;

namespace EducationProject2.Models.Abstract
{
    public interface IMongoDbObject
    {
        ObjectId Id { get; set; }
    }
}