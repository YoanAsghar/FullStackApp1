namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string passwordHash { get; set; } = string.Empty;
        public DateOnly Register_date { get; set; }

        public User(int id, string username, string email, string passwordHash, DateOnly Register_date)
        {
            this.Id = id;
            this.Username = username;
            this.Email = email;
            this.passwordHash = passwordHash;
            this.Register_date = Register_date;
        }
    }
}
