namespace Maxfire.Web.Mvc.FluentHtml
{
	public interface IElement
	{
		string TagName { get; }

		void AddCssClass(string @class);

		/// <summary>
		/// Query to find out if the attribute has been defined.
		/// </summary>
		/// <param name="name">The name of the attribute.</param>
		bool HasAttribute(string name);

		/// <summary>
		/// Set the value of the specified attribute.
		/// </summary>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		void SetAttr(string name, object value);

		/// <summary>
		/// Get the value of the specified attribute.
		/// </summary>
		/// <param name="name">The name of the attribute.</param>
		string GetAttr(string name);

		/// <summary>
		/// Remove an attribute.
		/// </summary>
		/// <param name="name">The name of the attribute to remove.</param>
		void RemoveAttr(string name);
	}
}