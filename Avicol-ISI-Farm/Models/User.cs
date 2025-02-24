namespace Avicol_ISI_Farm.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Created_At { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public ICollection<Orders> Orders { get; set; }
    }
}