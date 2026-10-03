using System;
using System.Net;
using System.Text.Json;
using PetInsurance.Exceptions;

namespace PetInsurance.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
 
    public GlobalExceptionMiddleware(RequestDelegate next,ILogger<GlobalExceptionMiddleware> logger)
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
            catch (BusinessRuleException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var response = new
            {
                Message = ex.Message
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
        //     _logger.LogError(ex,"Unhandled exception occurred");
        //     context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //     context.Response.ContentType = "application/json";
        //     var response = new
        //     {
        //     Message = "An unexpected error occurred."
        //     };
        //     await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        _logger.LogError(ex,
"Unhandled exception occurred");
 
context.Response.StatusCode = 500;
 
await context.Response.WriteAsJsonAsync(new
{
Message = ex.Message,
InnerException = ex.InnerException?.Message,
StackTrace = ex.StackTrace
});
         }
    }
}
