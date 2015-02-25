using System;
using Maxfire.Core.Reflection;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class EnumToTextFormatter<TEnum> : ValueFormatter<TEnum>
	{
		protected override string FormatValueCore(TEnum value)
		{
			Type enumType = typeof(TEnum);
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}

			return value.GetDisplayNameOfEnum();
		}
	}
}