using TaskManagement.src.Shared.Exceptions;

namespace TaskManagement.src.Presentation.Middlewares;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            context.Response.Clear();

            var statusCode = StatusCodes.Status500InternalServerError;
            var title = "An internal server error occurred.";
            string? detail = exception.Message;

            if (exception is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                title = "Resource not found.";
                detail = exception.Message;
            }
            else if (exception is InvalidStateException)
            {
                statusCode = StatusCodes.Status409Conflict;
                title = "Invalid state.";
                detail = exception.Message;
            }

            await Results.Problem(
                statusCode: statusCode, title: title, detail: detail).ExecuteAsync(context);
        }
    }
}
