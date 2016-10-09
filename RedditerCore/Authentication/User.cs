namespace RedditerCore.Authentication
{
    public class User
    {
        public User()
        {
            Username = null;
            Password = null;
            AccessToken = null;
        }

        public User(string username, string password) : this()
        {
            Username = username;
            Password = password;
        }

        public bool Authenticated => AccessToken != null;
        public string Username { get; set; }
        public string Password { get; set; }
        public Token AccessToken { get; set; }
    }
}
