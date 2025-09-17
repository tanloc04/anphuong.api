using System.Net;
using System.Text.Json;

namespace anphuong.api.Middlewares
{
    public class ErrorDetails
    {
        public static readonly ErrorDetails CAN_NOT_DELETE_YOURSELF = new(HttpStatusCode.Conflict, "Cannot Delete Yourself");
        public ErrorDetails(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public override string? ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
