using AuthServer.Models;
using MongoDB.Driver;

namespace AuthServer.Data.Interfaces
{
    public interface IUserContext
    {
        IMongoCollection<Users> Users { get; }
    }
}
