using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.Entities
{
    public class Behavior : Entity
    {
        [Key]
        public int ViewCount { get; set; }
        public int BuyCount { get; set; }
    }
}
