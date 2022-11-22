using MongoDB.Driver;
using sns_nrdb.Data.Repositories;

namespace sns_nrdb
{
    public class DbConnection
    {
        private readonly MongoClient client;
        private readonly IMongoDatabase database;

        public DbConnection(string connectionString)
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("SNS");

            Users = new UsersRepository(database);
            Posts = new PostsRepository(database);
        }

        public UsersRepository Users { get; }
        public PostsRepository Posts { get; }
    }
}