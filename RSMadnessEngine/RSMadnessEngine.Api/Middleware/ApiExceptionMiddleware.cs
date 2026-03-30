using Microsoft.AspNetCore.Mvc;
using RSMadnessEngine.Api.Errors;

namespace RSMadnessEngine.Api.Middleware
{
    public sealed class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
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
            catch (ApiException ex)
            {
                await WriteProblemAsync(
                    context,
                    ex.StatusCode,
                    ex.TypeUri,
                    TitleFor(ex.StatusCode),
                    ex.Detail,
                    ex.ErrorCode,
                    ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception.");
                await WriteProblemAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "https://rsmadness.app/problems/unexpected-error",
                    "Unexpected Error",
                    "An unexpected error occurred.",
                    "unexpected-error",
                    null);
            }
        }

        private static async Task WriteProblemAsync(
            HttpContext context,
            int status,
            string type,
            string title,
            string detail,
            string errorCode,
            IReadOnlyList<string>? errors)
        {
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Type = type,
                Title = title,
                Status = status,
                Detail = detail,
                Instance = context.Request.Path
            };

            problem.Extensions["errorCode"] = errorCode;
            problem.Extensions["traceId"] = context.TraceIdentifier;

            if (errors is { Count: > 0 })
            {
                problem.Extensions["errors"] = errors;
            }

            await context.Response.WriteAsJsonAsync(problem);
        }

        private static string TitleFor(int statusCode) => statusCode switch
        {
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status409Conflict => "Conflict",
            _ => "Error"
        };
    }
}
