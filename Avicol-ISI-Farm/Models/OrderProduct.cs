namespace Avicol_ISI_Farm.Models
{
    public class OrderProduct
    {
        public int OrderId { get; set; }
        public Orders Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
    }
}