using anphuong.Core.Domains.Objects;

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
