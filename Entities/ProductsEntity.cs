using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Column("product_name")]
        public string Name { get; set; } = string.Empty;
        [Column("prodct_description")]
        public string Description { get; set; } = string.Empty;
        [Column("product_price")]
        public float Price { get; set; }
        [Column("stock")]
        public int Stock { get; set; }

        public Product() { }

        public Product(string name, string Description, float price, int stock)
        {
            this.Name = name;
            this.Description = Description;
            this.Price = price;
            this.Stock = stock;
        }


    }
}
