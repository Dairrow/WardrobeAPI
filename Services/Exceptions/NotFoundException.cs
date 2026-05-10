namespace Wardrobe.Services.Exceptions;

public class NotFoundException
	: Exception
{
	public NotFoundException(
		string message)
		: base(message)
	{
	}
}