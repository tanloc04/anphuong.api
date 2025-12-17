using System.Net;
using anphuong.Core.Domains.Objects;

namespace anphuong.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public ErrorDetails Error { get; }

        public HttpStatusCode StatusCode => Error.StatusCode;

        public BusinessException(ErrorDetails error)
            : base(error.Message)
        {
            Error = error;
        }
    }
}
