namespace Application.ViewModels
#pragma warning disable 8618

{
    public class AuthViewModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public AuthViewModel(string accessToken, long userId, string username, string email)
        {
            RefreshToken = "Refresh tokens are not supported";
            AccessToken = accessToken;
            UserId = userId;
            Username = username;
            Email = email;
        }
    }
}