using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Maxfire.Core.Reflection
{
	/// <summary>
	/// The accessor (of a property) contains the executable statements associated with 
	/// getting (reading or computing) or setting (writing) the property.
	/// </summary>
	public interface Getter<TObject, TPropertyValue>
	{
		TPropertyValue GetValue(TObject target);
		string PropertyName { get; }
	}

	public interface Setter<TObject, TPropertyValue>
	{
		void SetValue(TObject target, TPropertyValue propertyValue);
		string PropertyName { get; }
	}

	public interface Accessor<TObject, TPropertyValue> : Getter<TObject, TPropertyValue>, Setter<TObject, TPropertyValue>
	{
	}

	public static class IntrospectionOf<TObject>
	{
		public static Getter<TObject, TPropertyValue> GetGetterFor<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			var propertyInfo = ExpressionHelper.GetProperty(expression);
			return new PropertyAccessor<TObject, TPropertyValue>(propertyInfo);
		}

		public static Setter<TObject, TPropertyValue> GetSetterFor<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			var propertyInfo = ExpressionHelper.GetProperty(expression);
			return new PropertyAccessor<TObject, TPropertyValue>(propertyInfo);
		}

		public static Accessor<TObject, TPropertyValue> GetAccessorFor<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			var propertyInfo = ExpressionHelper.GetProperty(expression);
			return new PropertyAccessor<TObject, TPropertyValue>(propertyInfo);
		}
	}

	public class PropertyAccessor<TObject, TPropertyValue> : Accessor<TObject, TPropertyValue>
	{
		private readonly PropertyInfo _propertyInfo;

		public PropertyAccessor(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		public void SetValue(TObject target, TPropertyValue propertyValue)
		{
			if (_propertyInfo.CanWrite)
			{
				_propertyInfo.SetValue(target, propertyValue, null);
			}
		}

		public TPropertyValue GetValue(TObject target)
		{
			return (TPropertyValue)_propertyInfo.GetValue(target, null);
		}

		public string PropertyName
		{
			get { return _propertyInfo.Name; }
		}
	}
}