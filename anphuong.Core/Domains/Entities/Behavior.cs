using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.Entities
{
    public class Behavior
    {
        [Key]
        public int BehaviorId { get; set; }
        public int ViewCount { get; set; }
        public int BuyCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
