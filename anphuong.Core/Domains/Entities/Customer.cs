namespace anphuong.Core.Domains.Entities
{
    public class Customer : Entity
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Behavior> Behaviors { get; set; }
    }
}
