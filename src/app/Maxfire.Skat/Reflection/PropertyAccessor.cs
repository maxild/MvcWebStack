using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Maxfire.Skat.Reflection
{
	/// <summary>
	/// The accessor (of a property) contains the executable statements associated with 
	/// getting (reading or computing) or setting (writing) the property.
	/// </summary>
	public interface Getter<in TObject, out TPropertyValue>
	{
		TPropertyValue GetValue(TObject target);
		string PropertyName { get; }
	}

	public interface Setter<in TObject, in TPropertyValue>
	{
		void SetValue(TObject target, TPropertyValue propertyValue);
		string PropertyName { get; }
	}

	public interface Accessor<in TObject, TPropertyValue> : Getter<TObject, TPropertyValue>, Setter<TObject, TPropertyValue>
	{
	}

	public static class IntrospectionOf<TObject>
	{
		public static Getter<TObject, TPropertyValue> GetGetterFor<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			var propertyInfo = GetProperty(expression);
			return new PropertyAccessor<TObject, TPropertyValue>(propertyInfo);
		}

		public static Setter<TObject, TPropertyValue> GetSetterFor<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			var propertyInfo = GetProperty(expression);
			return new PropertyAccessor<TObject, TPropertyValue>(propertyInfo);
		}

		public static Accessor<TObject, TPropertyValue> GetAccessorFor<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			var propertyInfo = GetProperty(expression);
			return new PropertyAccessor<TObject, TPropertyValue>(propertyInfo);
		}

		public static PropertyInfo GetProperty<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			MemberExpression memberExpression = getMemberExpression(expression);
			return (PropertyInfo)memberExpression.Member;
		}

		private static MemberExpression getMemberExpression<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			MemberExpression memberExpression = null;
			if (expression.Body.NodeType == ExpressionType.Convert)
			{
				var body = (UnaryExpression)expression.Body;
				memberExpression = body.Operand as MemberExpression;
			}
			else if (expression.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new ArgumentException("Not a member access", "expression");
			}

			return memberExpression;
		}
	}

	/// <summary>
	/// An anonymous type-safe accessor (getter and setter) for a property.
	/// </summary>
	/// <typeparam name="TObject">The type of object.</typeparam>
	/// <typeparam name="TPropertyValue">The type of property.</typeparam>
	///<remarks>
	/// Access restrictions are ignored for fully trusted code. That is, private properties can be accessed and mutated 
	/// via Reflection whenever the code is fully trusted.
	/// </remarks>
	public class PropertyAccessor<TObject, TPropertyValue> : Accessor<TObject, TPropertyValue>
	{
		private readonly PropertyInfo _propertyInfo;
		private const BindingFlags DEFAULT_BINDINGFLAGS = BindingFlags.Public | BindingFlags.Instance;

		public PropertyAccessor(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		public void SetValue(TObject target, TPropertyValue propertyValue)
		{
			_propertyInfo.SetValue(target, propertyValue, DEFAULT_BINDINGFLAGS, null, null, null);
		}

		public TPropertyValue GetValue(TObject target)
		{
			return (TPropertyValue)_propertyInfo.GetValue(target, DEFAULT_BINDINGFLAGS, null, null, null);
		}

		public string PropertyName
		{
			get { return _propertyInfo.Name; }
		}
	}
}