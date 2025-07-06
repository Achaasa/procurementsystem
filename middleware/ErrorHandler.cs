using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace procurementsystem.middleware
{
    public class ErrorHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandler> _logger;

        public ErrorHandler(RequestDelegate next, ILogger<ErrorHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            
            // Determine status code based on exception type
            var (statusCode, message) = GetStatusCodeAndMessage(exception);
            response.StatusCode = (int)statusCode;

            // Extract file and line number from stack trace (best effort)
            var stackTrace = new StackTrace(exception, true);
            var frame = stackTrace.GetFrames()?.FirstOrDefault(f => !string.IsNullOrEmpty(f.GetFileName()));

            var errorDetails = new
            {
                message = message,
                exceptionType = exception.GetType().Name,
                source = exception.Source,
                file = frame?.GetFileName(),
                line = frame?.GetFileLineNumber(),
                method = frame?.GetMethod()?.Name,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path,
                hint = GetHelpfulHint(exception)
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true // Makes the output easy to read
            };

            var json = JsonSerializer.Serialize(errorDetails, options);
            return response.WriteAsync(json);
        }

        private static (HttpStatusCode statusCode, string message) GetStatusCodeAndMessage(Exception ex)
        {
            return ex switch
            {
                ArgumentNullException => (HttpStatusCode.BadRequest, ex.Message),
                InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
                ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
                DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "The record was modified by another user. Please refresh and try again."),
                DbUpdateException dbEx => (HttpStatusCode.BadRequest, $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}"),
                SocketException => (HttpStatusCode.ServiceUnavailable, "Database connection failed. Please check your database connection."),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };
        }

        private static string GetHelpfulHint(Exception ex)
        {
            return ex switch
            {
                ArgumentNullException argNull => $"The argument '{argNull.ParamName}' was null. Make sure it is provided.",
                InvalidOperationException => "Something is being used in an invalid state. Check initialization or service registrations.",
                KeyNotFoundException => "A lookup failed. Verify if the requested key exists.",
                DbUpdateConcurrencyException => "The data was modified by another user. Please refresh and try again.",
                DbUpdateException => "Database operation failed. Check your data and try again.",
                SocketException => "Database connection failed. Check if your database server is running and the connection string is correct.",
                ArgumentException => "Invalid argument provided. Check your input data.",
                _ => "Check the stack trace and source location to debug this issue."
            };
        }
    }
}