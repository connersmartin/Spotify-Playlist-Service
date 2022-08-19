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
        private readonly IConfiguration _configuration;
        private string _mongoSettings;
        private string _dbName;
        private MongoClientSettings _settings;
        private MongoClient _client;
        private IMongoDatabase _database;
        public MongoProvider(IConfiguration configuration)
        {
            _mongoSettings = configuration.GetValue<string>("MongoSettings");
            _dbName = configuration.GetValue<string>("DatabaseName");
            _settings = MongoClientSettings.FromConnectionString(_mongoSettings);
            _client = new MongoClient(_settings);
            _database = _client.GetDatabase(_dbName);
            _configuration = configuration;
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
            var filter = Builders<BsonDocument>.Filter.Eq("userId", id);

            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var doc = await collection.FindAsync(filter);

            var list = doc.ToList();

            var data = Helper<T>(list);

            return data;
        }

        /// <summary>
        /// Adds a document to a given collection using json
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="infoJson">string of json</param>
        /// <returns></returns>
        public async Task Add(string collectionName, string infoJson)
        {
            var doc = BsonDocument.Parse(infoJson);

            var collection = _database.GetCollection<BsonDocument>(collectionName);
            await collection.InsertOneAsync(doc);
        }

        public async Task Delete(string collectionName, string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));

            var collection = _database.GetCollection<BsonDocument>(collectionName);

            await collection.DeleteOneAsync(filter);
        }

        //What else do we need?
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
