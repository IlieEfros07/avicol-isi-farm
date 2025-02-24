namespace Avicol_ISI_Farm.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; }
        public DateTime DateAdded { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}