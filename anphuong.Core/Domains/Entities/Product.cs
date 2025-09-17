using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.Entities
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; }
        public int LongSize { get; set; }
        public int WidthSize { get; set; }
        public int HeightSize { get; set; }
        public string Material { get; set; }
        public int DetailImageId { get; set; }
        public int CategoryId { get; set; }
        public int VariationId { get; set; }
        public Variant Variant { get; set; }
        public DetailImage DetailImage { get; set; }
        public Category Category { get; set; }
    }
}
