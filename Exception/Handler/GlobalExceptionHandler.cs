using System.Text.Json;
using Task_System.Exception.IssueException;
using Task_System.Exception.ProjectException;
using Task_System.Exception.UserException;

namespace Task_System.Exception.Handler
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    UserNotFoundException => StatusCodes.Status404NotFound,
                    UserAlreadyExistsException => StatusCodes.Status409Conflict,
                    IssueCreationException => StatusCodes.Status400BadRequest,
                    ProjectNotFoundException => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };
                var errorResponse = new Task_System.Exception.Error.ErrorResponse(
                    ex switch
                    {
                        UserNotFoundException => Task_System.Exception.Error.ErrorType.USER_NOT_FOUND,
                        UserAlreadyExistsException => Task_System.Exception.Error.ErrorType.USER_ALREADY_REGISTERED,
                        IssueCreationException => Task_System.Exception.Error.ErrorType.ISSUE_CREATION_ERROR,
                        ProjectNotFoundException => Task_System.Exception.Error.ErrorType.PROJECT_NOT_FOUND,
                        _ => Task_System.Exception.Error.ErrorType.SERVER_ERROR
                    },
                    ex.Message
                );
                var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
                {
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                });
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
