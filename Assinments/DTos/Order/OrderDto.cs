namespace Assinments.DTos.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }
        public List<string> Items { get; set; } = new List<string>();

    }
}
