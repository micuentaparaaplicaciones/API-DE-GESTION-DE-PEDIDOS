using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client; // Oracle-specific
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally in the API. 
    /// </summary>
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = MapStatusCode(ex);

                var response = new
                {
                    message = GetFriendlyMessage(ex),
                    detail = context.RequestServices
                        .GetRequiredService<IHostEnvironment>().IsDevelopment()
                        ? ex.ToString()
                        : null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }

        private static int MapStatusCode(Exception ex) => ex switch
        {
            DbUpdateConcurrencyException => (int)HttpStatusCode.Conflict,
            DbUpdateException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            SqlException => (int)HttpStatusCode.InternalServerError, // SQL Server-specific
            OracleException => (int)HttpStatusCode.InternalServerError, // Oracle-specific
            _ => (int)HttpStatusCode.InternalServerError
        };

        private static string GetFriendlyMessage(Exception ex) => ex switch
        {
            DbUpdateConcurrencyException => "The data was modified by another user. Please reload and try again.",
            DbUpdateException => "A database update error occurred. Check your data.",
            InvalidOperationException => "Invalid operation. Please verify the request.",
            ArgumentException => "One or more arguments are invalid.",
            SqlException => "A SQL Server error occurred. Please try again later.",
            OracleException oracleEx => GetOracleMessage(oracleEx),
            _ => "An unexpected error occurred. Please try again later."
        };

        private static string GetOracleMessage(OracleException ex)
        {
            return ex.Number switch
            {
                1 => "Unique constraint violation (e.g. duplicate IDENTIFICATION or EMAIL).",
                2291 => "Foreign key constraint failed. A referenced user may not exist.",
                1400 => "A required field is missing.",
                _ => "A database error occurred. Please try again later."
            };
        }
    }
}
