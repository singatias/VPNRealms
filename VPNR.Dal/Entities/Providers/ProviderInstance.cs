using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PoweredSoft.ObjectStorage.MongoDB;

namespace VPNR.Dal.Entities.Providers
{
    [MongoCollection("ProviderInstances"), BsonIgnoreExtraElements]
    [BsonDiscriminator(RootClass = true), BsonKnownTypes(typeof(LinodeInstance))]
    public class ProviderInstance
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Ipv4 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}