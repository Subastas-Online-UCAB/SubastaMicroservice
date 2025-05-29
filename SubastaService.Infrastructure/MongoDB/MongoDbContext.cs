using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SubastaService.Infrastructure.MongoDB;
using SubastaService.Infrastructure.MongoDB.Documents;

namespace SubastaService.Infrastructure.Mongo
{
    public class MongoDbContext : ISubastaMongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoSettings> options)
        {
            var settings = options.Value;
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<SubastaDocument> Subastas =>
            _database.GetCollection<SubastaDocument>("subastas");

        public IMongoDatabase Database => _database;
    }
}
