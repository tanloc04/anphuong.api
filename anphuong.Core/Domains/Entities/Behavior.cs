namespace anphuong.Core.Domains.Entities
{
    public class Behavior : Entity
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public int Count { get; set; }
        public Customer Customer { get; set; }
        public Product Product { get; set; }

    }
}
