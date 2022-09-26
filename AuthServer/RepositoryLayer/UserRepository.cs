using AuthServer.Data.Interfaces;
using AuthServer.RepositoryLayer.Interfaces;
using MongoDB.Driver;

namespace AuthServer.RepositoryLayer
{
    public class UserRepository : IUserRepository
    {
        private readonly IUserContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IUserContext context, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ValidateUser(string? userName, string? password)
        {
            try
            {
                var response = await _context.Users.Find(x => x.UserName == userName && x.Password == password).FirstOrDefaultAsync();
                if (response != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some internal error happened while fetching user details from DB.");
                throw;
            }
        }
    }
}
