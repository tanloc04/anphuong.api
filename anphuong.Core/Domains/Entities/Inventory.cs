using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.Entities
{
    public class Inventory : Entity
    {
        [Key]
        public int QuantityInStock { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
