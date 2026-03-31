using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.Entities
{
    public class ProductReview : Entity
    {
        public int ProductId { get; set; }
        public string? UserId { get; set; }
        public string ReviewerName { get; set; }
        public int RatingValue { get; set; }
        public string? Comment { get; set; }
        public virtual Product Product { get; set; }
    }
}
