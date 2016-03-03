namespace Maxfire.Core
{
	public interface ITextValuePair<TValue>
	{
		string Text { get; }
		TValue Value { get; }
	}

	public interface ITextValuePair : ITextValuePair<string>
	{
	}
}