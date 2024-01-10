namespace dotnet_core_auth_api.Models.Users
{
    public class UserLogin
    {
        public string Email { get; set; } = string.Empty;
        public string JWT { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsPersonalTrainer { get; set; } = false;
    }
}
