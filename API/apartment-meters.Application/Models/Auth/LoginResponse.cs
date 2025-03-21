namespace Application.Models.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime Expiration { get; set; }
    }
} 