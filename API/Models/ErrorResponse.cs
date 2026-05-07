namespace Wardrobe.API.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }

    public string Message { get; set; } = null!;

    public string TraceId { get; set; } = null!;

    public DateTime TimestampUtc { get; set; }
}