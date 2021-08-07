using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApiServer.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get; set;}

        [BsonElement("Name")]
        public string UserName {get; set;}

        public int Age { get; set; }
    }
}