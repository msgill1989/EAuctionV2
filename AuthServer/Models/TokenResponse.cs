namespace AuthServer.Models
{
    public class TokenResponse
    {
        public string? Token { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
