using AuthServer.Models;

namespace AuthServer.Token.Interface
{
    public interface ITokenService
    {
        TokenResponse BuildToken(string key, string issuer, IEnumerable<string> audience, string userName);
    }
}
