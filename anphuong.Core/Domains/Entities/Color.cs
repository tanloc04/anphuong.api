namespace anphuong.Core.Domains.Entities
{
    public class Color : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string HexCode { get; set; } = string.Empty;
        public ICollection<Variant> Variants { get; set; } = null!;
    }
}
