using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PoweredSoft.ObjectStorage.MongoDB;

namespace VPNR.Dal.Entities.Nodes
{
    [MongoCollection("Nodes"), BsonIgnoreExtraElements]
    [BsonDiscriminator(RootClass = true), BsonKnownTypes(typeof(WireguardNode))]
    public class Node
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string InstanceId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}