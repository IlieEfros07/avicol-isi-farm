namespace Avicol_ISI_Farm.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string Adress { get; set; }

        public User User { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}