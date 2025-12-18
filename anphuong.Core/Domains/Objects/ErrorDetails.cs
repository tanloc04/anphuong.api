using System.Net;
using System.Text.Json;

namespace anphuong.Core.Domains.Objects
{
    public class ErrorDetails
    {
        #region Error Detail

        /*Add ErrorDetails here, for example:
        public static readonly ErrorDetails INVALID_ID = new(HttpStatusCode.BadRequest, "Invalid ID");*/

        #endregion

        public ErrorDetails(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        //Common
        public static readonly ErrorDetails DEFAULT =
            new(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
        public static readonly ErrorDetails CAN_NOT_DELETE_YOURSELF =
            new(HttpStatusCode.Forbidden, "Cannot delete yourself!");
        public static readonly ErrorDetails INVALID_ID =
            new(HttpStatusCode.BadRequest, "Invalid id!");
        public static readonly ErrorDetails ID_NOT_FOUND =
            new(HttpStatusCode.NotFound, "Id not found!");
        public static readonly ErrorDetails NO_ITEM =
            new(HttpStatusCode.NotFound, "Please choose atleast 1 product!");
        public static readonly ErrorDetails OUT_OF_STOCK =
            new(HttpStatusCode.NotFound, "Out of stock");
        public static readonly ErrorDetails INVALID_CATEGORY_ID =
            new(HttpStatusCode.NotFound, "Category Id dont exist!");

        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public override string? ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
