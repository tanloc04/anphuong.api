using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.Entities
{
    public class User : Entity
    {
        [Key]
        public string Username { get; set; }
        public string? Password { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
