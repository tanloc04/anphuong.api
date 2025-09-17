namespace anphuong.Core.Domains.Entities
{
    public class Customer : Entity
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string CustomerAddress { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
