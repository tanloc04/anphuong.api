using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.Entities
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
