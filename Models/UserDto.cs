namespace Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string passwordHash { get; set; } = string.Empty;
        public DateOnly Register_date { get; set; }
    }

}
