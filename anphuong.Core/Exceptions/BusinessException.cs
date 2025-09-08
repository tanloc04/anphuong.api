using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using anphuong.Core.Domains.Objects;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace anphuong.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public ErrorDetails Error { get; set; } = null!;

        public BusinessException(ErrorDetails error)
        {
            Error = error;
        }
        public BusinessException()
        {

        }
    }
}
