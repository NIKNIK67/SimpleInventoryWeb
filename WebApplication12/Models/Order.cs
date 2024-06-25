namespace WebApplication12.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<Item> Items { get; set; }
        public DateTime DateTime { get; set; }
    }
}
