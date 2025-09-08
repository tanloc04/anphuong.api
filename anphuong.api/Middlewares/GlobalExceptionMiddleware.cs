using System.Net;
using anphuong.Core.Domains.DTOs.API;
using anphuong.Core.Exceptions;

namespace anphuong.api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                _logger.LogError($"A new business exception has been thrown: {ex}");
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            int statusCode;
            string message;
            switch (exception)
            {
                case BusinessException businessException:
                    statusCode = (int)businessException.Error.StatusCode;
                    message = businessException.Error.Message;
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = exception.Message;
                    break;
            }
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(new APIResponseDTO<object>
            {
                Success = false,
                Message = message
            }.ToString() ?? string.Empty);
        }
    }
}
