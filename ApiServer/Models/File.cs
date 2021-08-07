using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ApiServer.Models
{
    public class File
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get; set;}

        public string Name {get; set;}
        public string OriginalName {get; set;}

        public string Url { get; set; }
        public string Type {get; set;}
        public string GeneralType {get; set;}
        public Int64 Size {get; set;}
        public string TimeStamp {get; set;}

    }
}