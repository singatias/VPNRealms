using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PoweredSoft.ObjectStorage.Core;
using PoweredSoft.ObjectStorage.MongoDB;
using VPNR.Dal.Entities;
using VPNR.Dal.Entities.Nodes;
using VPNR.Dal.Entities.Providers;

namespace VPNR.Dal
{
    public class DbContext : MongoObjectStorageContext
    {
        public IObjectStorageCollection<User> Users => GetCollection<User>();
        
        public IObjectStorageCollection<Node> Nodes => GetCollection<Node>();
        public IObjectStorageCollection<WireguardNode> WireguardNodes => GetCollection<WireguardNode>();
        
        public IObjectStorageCollection<ProviderInstance> ProviderInstances => GetCollection<ProviderInstance>();
        public IObjectStorageCollection<LinodeInstance> LinodeInstances => GetCollection<LinodeInstance>();

        static DbContext()
        {
            // @QUESTION what this do? support of 128 bit decimal for MongoDB?
            BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
        }
        
        public DbContext(IConfiguration configuration) : base(GetDatabase(configuration))
        {
        }
        
        private static IMongoDatabase GetDatabase(IConfiguration configuration)
        {
            var connectionString = configuration["Mongo:ConnectionString"];
            var dbName = configuration["Mongo:Database"];
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(dbName);
            return db;
        }
        
        private IObjectStorageCollection<TChild> GetInheritedCollection<TParent, TChild>()
            where TChild : TParent 
        {
            var attribute = typeof(TParent).GetCustomAttribute<MongoCollectionAttribute>();
            if (attribute == null)
                throw new Exception("Must add MongoCollectionAttribute on entity class to use this method.");

            var mongoCollection = Database.GetCollection<TChild>(attribute.Name).OfType<TChild>();
            var ret = new MongoObjectStorageCollection<TChild>(mongoCollection);
            return ret;
        }
    }
}