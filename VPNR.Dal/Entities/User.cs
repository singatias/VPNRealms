using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PoweredSoft.ObjectStorage.MongoDB;

namespace VPNR.Dal.Entities
{
    [MongoCollection("Users"), BsonIgnoreExtraElements]
    public class User
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string KeycloakUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}