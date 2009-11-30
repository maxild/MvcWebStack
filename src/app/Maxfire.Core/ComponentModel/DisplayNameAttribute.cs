using System;

namespace Maxfire.Core.ComponentModel
{
	// System.ComponentModel.DisplayName is not allowed on fields, and can therefore not be used on enum fields

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class DisplayNameAttribute : Attribute
	{
		public DisplayNameAttribute() : this(string.Empty)
		{
		}

		public DisplayNameAttribute(string displayText)
		{
			DisplayName = displayText;
		}

		public string DisplayName { get; set; }

		//public override bool Equals(object obj)
		//{
		//    if (obj == this)
		//    {
		//        return true;
		//    }
		//    var displayNameAttribute = obj as DisplayNameAttribute;
		//    return ((displayNameAttribute != null) && (displayNameAttribute.DisplayName == DisplayName));
		//}

		//public override int GetHashCode()
		//{
		//    return DisplayName.GetHashCode();
		//}
	}
}