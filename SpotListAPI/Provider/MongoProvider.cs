using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace SpotListAPI.Provider
{
    public class MongoProvider
    {
        private string _mongoSettings;
        private string _dbName;
        private MongoClientSettings _settings;
        private MongoClient _client;
        private IMongoDatabase _database;
        public MongoProvider(IConfiguration configuration)
        {
            _mongoSettings = configuration.GetValue<string>("mongoSettings");
            _dbName = configuration.GetValue<string>("dbName");
            _settings = MongoClientSettings.FromConnectionString(_mongoSettings);
            _client = new MongoClient(_settings);
            _database = _client.GetDatabase(_dbName);
        }
        public async Task<List<T>> GetAll<T>(string collectionName)
        {     
            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var doc = await collection.FindAsync(new BsonDocument());

            var list = doc.ToList();

            var data = Helper<T>(list);

            return data;
        }

        public async Task<List<T>> GetOne<T>(string collectionName, string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));

            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var doc = await collection.FindAsync(filter);

            var list = doc.ToList();

            var data = Helper<T>(list);

            return data;
        }

        public async Task Add<T>(string collectionName, T info)
        {
            //maybe have this just come in as json?!?
            var infoJson = info.ToJson();
            var doc = BsonDocument.Parse(infoJson);

            var collection = _database.GetCollection<BsonDocument>(collectionName);
            await collection.InsertOneAsync(doc);
        }

        //What else do we need?
        //Delete option prob
        //FindOneAndUpdate Maybe?

        public static List<T> Helper<T>(List<BsonDocument> list)
        {
            var data = new List<T>();
            foreach(var l in list)
            {
                data.Add(BsonSerializer.Deserialize<T>(l));
            }
            return data;
        }
    }
}
