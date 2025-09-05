using System.Text.Json;

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
                    Task_System.Exception.UserException.UserNotFoundException => StatusCodes.Status404NotFound,
                    Task_System.Exception.UserException.UserAlreadyExistsException => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError
                };
                var errorResponse = new Task_System.Exception.Error.ErrorResponse(
                    ex switch
                    {
                        Task_System.Exception.UserException.UserNotFoundException => Task_System.Exception.Error.ErrorType.USER_NOT_FOUND,
                        Task_System.Exception.UserException.UserAlreadyExistsException => Task_System.Exception.Error.ErrorType.USER_ALREADY_REGISTERED,
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
