namespace anphuong.Core.Domains.Entities
{
    public class DetailImage : Entity
    {
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
