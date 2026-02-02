namespace Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Stock { get; set; }


        public Product(int id, string name, string Description, float price, int stock)
        {
            this.Id = id;
            this.Name = name;
            this.Description = Description;
            this.Price = price;
            this.Stock = stock;
        }


    }
}
