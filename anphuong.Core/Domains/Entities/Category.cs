namespace anphuong.Core.Domains.Entities
{
    public class Category : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
