using Microsoft.AspNetCore.Http;

namespace RSMadnessEngine.Api.Errors
{
    public abstract class ApiException : Exception
    {
        protected ApiException(int statusCode, string errorCode, string detail, IEnumerable<string>? errors = null)
            : base(detail)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Detail = detail;
            Errors = errors?.ToArray() ?? Array.Empty<string>();
        }

        public int StatusCode { get; }
        public string ErrorCode { get; }
        public string Detail { get; }
        public IReadOnlyList<string> Errors { get; }

        public string TypeUri => $"https://rsmadness.app/problems/{ErrorCode}";
    }

    public sealed class ApiValidationException : ApiException
    {
        public ApiValidationException(string errorCode, string detail, IEnumerable<string> errors)
            : base(StatusCodes.Status400BadRequest, errorCode, detail, errors) { }
    }

    public sealed class ApiNotFoundException : ApiException
    {
        public ApiNotFoundException(string errorCode, string detail)
            : base(StatusCodes.Status404NotFound, errorCode, detail) { }
    }

    public sealed class ApiConflictException : ApiException
    {
        public ApiConflictException(string errorCode, string detail)
            : base(StatusCodes.Status409Conflict, errorCode, detail) { }
    }
}