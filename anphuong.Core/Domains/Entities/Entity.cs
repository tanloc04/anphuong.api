namespace anphuong.Core.Domains.Entities
{
    public abstract class Entity
    {
        public virtual int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        protected Entity()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            IsDeleted = false;
        }
    }
}
