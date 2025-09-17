using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.Entities
{
    public class Customer : Entity
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string CustomerAddress { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
