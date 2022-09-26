using AuthServer.Models;

namespace AuthServer.RepositoryLayer.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ValidateUser(string userName, string password);
    }
}
