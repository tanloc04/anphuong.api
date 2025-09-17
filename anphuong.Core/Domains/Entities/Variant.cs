using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.Entities
{
    public class Variant
    {
        [Key]
        public int VariantId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public int ColorId { get; set; }
        public int ProductId { get; set; }
        public Color Color { get; set; }
        public Product Product { get; set; }
    }
}
