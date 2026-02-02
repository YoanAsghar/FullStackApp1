namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateOnly Register_date { get; set; }

        public User(int id, string username, string email, string password, DateOnly Register_date)
        {
            this.Id = id;
            this.Username = username;
            this.Email = email;
            this.Password = password;
            this.Register_date = Register_date;
        }
    }
}
