using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL.Services.Singleton;
using WaterFilterBusiness.Common.ErrorHandling.Exceptions;

namespace WaterFilterBusiness.API.Common;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILoggerService _loggerService;

    public ExceptionHandlingMiddleware(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            ProblemDetails problemDetails;

            if (ex is BaseException)
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad request",
                    Detail = ex.Message
                };
            else
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server error"
                };

            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails);

            _loggerService.LogError(ex);
        }
    }
}