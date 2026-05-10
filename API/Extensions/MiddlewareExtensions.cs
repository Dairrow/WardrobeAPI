using Wardrobe.API.Middleware;

namespace Wardrobe.API.Extensions;

public static class MiddlewareExtensions
{
	public static IApplicationBuilder
		UseGlobalExceptionHandling(
			this IApplicationBuilder app)
	{
		app.UseMiddleware<
			ExceptionMiddleware>();


		return app;
	}
}