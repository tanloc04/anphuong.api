using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public override string? ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
