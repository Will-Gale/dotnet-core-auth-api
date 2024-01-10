namespace dotnet_core_auth_api.Models.Users
{
    public class UserProfileDTO
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string UserId { get; set; }
    }
}
