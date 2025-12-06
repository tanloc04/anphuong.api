using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Exceptions
{
    public class GoogleException
    {
        public class TokenExpiredException : Exception
        {
            public TokenExpiredException(string message) : base(message) { }
        }

        public class InvalidTokenException : Exception
        {
            public InvalidTokenException(string message) : base(message) { }
        }
    }
}
