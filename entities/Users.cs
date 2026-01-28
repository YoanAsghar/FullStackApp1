namespace Entities
{
    public class user
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public List<Product> Cart { get; set; }

        public user(int id, string username, string email, string hashedPassword, List<Product> cart)
        {
            this.Id = id;
            this.Username = username;
            this.Email = email;
            this.HashedPassword = hashedPassword;
            this.Cart = cart;
        }


    }
}
