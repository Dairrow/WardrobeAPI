using System.Text.Json;
using Wardrobe.API.Models;
using Wardrobe.Services.Exceptions;

namespace Wardrobe.API.Middleware;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;

	private readonly ILogger<ExceptionMiddleware>
		_logger;


	public ExceptionMiddleware(
		RequestDelegate next,
		ILogger<ExceptionMiddleware> logger)
	{
		_next = next;

		_logger = logger;
	}


	public async Task InvokeAsync(
		HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception exception)
		{
			await HandleExceptionAsync(
				context,
				exception);
		}
	}


	private async Task HandleExceptionAsync(
		HttpContext context,
		Exception exception)
	{
		_logger.LogError(
			exception,
			"Unhandled exception");


		var statusCode =
			exception switch
			{
				NotFoundException =>
					StatusCodes.Status404NotFound,

				ConflictException =>
					StatusCodes.Status409Conflict,

				UnauthorizedException =>
					StatusCodes.Status401Unauthorized,

				ValidationException =>
					StatusCodes.Status400BadRequest,

				_ =>
					StatusCodes
						.Status500InternalServerError
			};


		var response =
			new ErrorResponse
			{
				StatusCode = statusCode,

				Message = exception.Message,

				TraceId =
					context.TraceIdentifier,

				TimestampUtc =
					DateTime.UtcNow
			};


		context.Response.ContentType =
			"application/json";

		context.Response.StatusCode =
			statusCode;


		await context.Response.WriteAsync(
			JsonSerializer.Serialize(
				response));
	}
}