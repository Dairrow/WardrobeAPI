using Serilog;

namespace Wardrobe.API.Extensions;

public static class LoggingExtensions
{
	public static WebApplicationBuilder
		AddSerilogLogging(
			this WebApplicationBuilder builder)
	{
		Log.Logger =
			new LoggerConfiguration()
				.ReadFrom
				.Configuration(
					builder.Configuration)
				.Enrich
				.FromLogContext()
				.CreateLogger();


		builder.Host
			.UseSerilog();


		return builder;
	}
}