using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.Common.Exceptions;

namespace WaterFilterBusiness.API.Common;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    // TODO add logger

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            ProblemDetails problemDetails;

            switch (ex)
            {
                case BaseException:
                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Bad request",
                        Detail = ex.Message
                    };
                    break;
                default:
                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Server error"
                    };
                    break;

            }

            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}