using LocksApi.UseCases.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LocksApi;

public class ResponseStatusCodeMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseStatusCodeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (NotFoundApplicationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (InvalidOperationException e) when (e.InnerException is DbUpdateException {InnerException: PostgresException {SqlState: "40001"}})//could not serialize access due to concurrent update
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        }
    }
}