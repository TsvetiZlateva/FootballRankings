using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FootballRankings.Api.Common
{
    public class ExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
        {
            try
            {
                await next(ctx);
            }
            catch (ValidationException vex)
            {
                var problemDetails = new ValidationProblemDetails(
                    vex.Errors.GroupBy(e => e.PropertyName)
                              .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
                { Status = StatusCodes.Status400BadRequest, Title = "Validation failed" };

                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "application/problem+json";
                await ctx.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (DbUpdateConcurrencyException)
            {
                var problemDetails = new ProblemDetails { Status = 409, Title = "Concurrency conflict", Detail = "Resource was modified by someone else." };
                ctx.Response.StatusCode = 409;
                ctx.Response.ContentType = "application/problem+json";
                await ctx.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (KeyNotFoundException ex)
            {
                var problemDetails = new ProblemDetails { Status = 404, Title = "Not found", Detail = ex.Message };
                ctx.Response.StatusCode = 404;
                ctx.Response.ContentType = "application/problem+json";
                await ctx.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (InvalidOperationException ex)
            {
                var problemDetails = new ProblemDetails { Status = 400, Title = "Invalid operation", Detail = ex.Message };
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "application/problem+json";
                await ctx.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (SqlException ex)
            {
                var problemDetails = new ProblemDetails { Status = 500, Title = "Database error", Detail = ex.Message };
                ctx.Response.StatusCode = 500;
                ctx.Response.ContentType = "application/problem+json";
                await ctx.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails { Status = 500, Title = "Unexpected error", Detail = ex.Message };
                ctx.Response.StatusCode = 500;
                ctx.Response.ContentType = "application/problem+json";
                await ctx.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
