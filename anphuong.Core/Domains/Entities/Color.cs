namespace anphuong.Core.Domains.Entities
{
    public class Color : Entity
    {
        public string Name { get; set; }
        public string HexCode { get; set; }
        public ICollection<Variant> Variants { get; set; }
    }
}
