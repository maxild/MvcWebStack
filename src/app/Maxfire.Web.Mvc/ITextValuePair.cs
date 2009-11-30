namespace Maxfire.Web.Mvc
{
	public interface ITextValuePair<T>
	{
		string Text { get; }
		T Value { get; }
	}

	public interface ITextValuePair : ITextValuePair<string>
	{
	}
}