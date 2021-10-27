using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApi.Middlewares
{
    public class ExceptionResult
    {
        public string Error { get; set; }
    }

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize<ExceptionResult>(new ExceptionResult() { Error = exception.Message });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            context.Response.Headers.Remove("Cache-Control");
            context.Response.Headers.Remove("Expires");
            context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            context.Response.Headers.Add("Expires", "-1");
            return context.Response.WriteAsync(result);
        }

        
    }
}