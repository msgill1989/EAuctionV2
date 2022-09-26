using AuthServer.Data.Interfaces;
using AuthServer.Models;
using MongoDB.Driver;

namespace AuthServer.Data
{
    public class UserContext : IUserContext
    {
        public UserContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("UserDatabase:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("UserDatabase:DatabaseName"));

            Users = database.GetCollection<Users>(configuration.GetValue<string>("UserDatabase:UserCollectionName"));
        }

        public IMongoCollection<Users> Users { get; }
    }
}
