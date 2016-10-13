using System.Threading.Tasks;
using LinqUsageAnalyzer.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LinqUsageAnalyzer.DAL
{
    class MongoStatisticsRepository : IStatisticsRepository
    {
        private const string CollectionName = "Statistics";
        private readonly StatisticsMapperWithLinqData _mapper;
        private readonly IMongoDatabase _database;

        public MongoStatisticsRepository(string databaseName)
        {
            _mapper = new StatisticsMapperWithLinqData();
            BsonClassMap.RegisterClassMap<StatisticsDO>();

            IMongoClient client = new MongoClient();

            _database = client.GetDatabase(databaseName);
        }

        public async Task SaveAsync(RepositoryStatistics statistics)
        {
            var dataObject = _mapper.CreateDataObject(statistics);

            var bsonObject = dataObject.ToBsonDocument();

            var collection = _database.GetCollection<BsonDocument>(CollectionName);

            await collection.InsertOneAsync(bsonObject);
        }

        public bool RepositoryExist(string repositoryName)
        {
            var collection = _database.GetCollection<StatisticsDO>(CollectionName);

            return collection.AsQueryable()
                .Where(data => data.Name == repositoryName)
                .Any();
        }
    }
}