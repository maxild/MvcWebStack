using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Maxfire.Core.Extensions;

namespace Maxfire.Core.Reflection
{
	public class ExpressionNameVisitor
	{
		protected StringBuilder Builder { get; private set; }
		protected bool LambdaParameterHasBeenSeen { get; private set; }

		public string ToString(Expression expression)
		{
			if (expression == null)
			{
				return string.Empty;
			}

			Builder = new StringBuilder();
			LambdaParameterHasBeenSeen = false;
			try
			{
				Visit(expression);
				string expressionName = Builder.ToString();
				// Remove Model at the front of the expression name unless lambda parameter
				// has been used (we do not want to remove Model property of Model)
				if (!LambdaParameterHasBeenSeen && expressionName.StartsWith(".model", StringComparison.OrdinalIgnoreCase))
				{
					expressionName = expressionName.Substring(6, expressionName.Length - 6);
				}
				return expressionName.TrimStart('.');
			}
			finally
			{
				Builder = null;
			}
		}

		private void Visit(Expression expression)
		{
			if (expression is UnaryExpression)
			{
				Visit(((UnaryExpression)expression).Operand);
			}
			else if (expression is BinaryExpression && expression.NodeType == ExpressionType.ArrayIndex)
			{
				VisitArrayIndex((BinaryExpression)expression);
			}
			else if (expression is ConstantExpression)
			{
				VisitConstant((ConstantExpression)expression);
			}
			else if (expression is MemberExpression) {
				VisitMemberAccess((MemberExpression)expression);
			}
			else if (expression is MethodCallExpression)
			{
				VisitMethodCall((MethodCallExpression)expression);
			}
			else if (expression is LambdaExpression)
			{
				VisitLambda((LambdaExpression)expression);
			}
			else if (expression is ParameterExpression)
			{
				VisitParameter((ParameterExpression)expression);
			}
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private void VisitParameter(ParameterExpression expression)
		{
			// When the expression is parameter based (m => m.Something...), we remember
			// it to make sure that we do not accidentally cut off too much of m => m.Model....
			LambdaParameterHasBeenSeen = true;
		}

		private void VisitLambda(LambdaExpression expression)
		{
			Visit(expression.Body);
		}

		private void VisitArrayIndex(BinaryExpression expression)
		{
			Visit(expression.Left);
			Builder.Append("[");
			VisitMethodCallArgument(expression.Right);
			Builder.Append("]");
		}

		private void VisitConstant(ConstantExpression expression)
		{
			// Captured references declared as compiler generated types of closures are excluded
			if (expression.Type.HasCustomAttribute<CompilerGeneratedAttribute>())
			{
				return;
			}
			var index = Expression.Lambda(expression).Compile().DynamicInvoke();
			Builder.Append(index.ToNullSafeString());
		}

		private void VisitMemberAccess(MemberExpression expression)
		{
			if (expression.Expression != null)
			{
				Visit(expression.Expression);
			}
			else
			{
				Builder.Append(expression.Member.DeclaringType?.Name);
			}
			// In case of m => m.BirthDay.Value.Day, where BirthDay is declared as Nullable<DateTime>,
			// we want BirthDay.Day (as if BirthDay is not Nullable<DateTime>).
			if (false == expression.Member.DeclaringType.IsNullableValueType())
			{
				// We always prepend dot operator and remember to clean up before returning final result
				Builder.Append(".");
				Builder.Append(expression.Member.Name);
			}
		}

		private void VisitMethodCall(MethodCallExpression expression)
		{
			Visit(expression.Object);

			if (expression.IsIndexerProperty() || expression.IsArrayGetMethod())
			{
				Builder.Append("[");
				VisitMethodCallArguments(expression);
				Builder.Append("]");
			}
			else
			{
				Builder.Append(".");
				Builder.Append(expression.Method.Name);
				Builder.Append("(");
				VisitMethodCallArguments(expression);
				Builder.Append(")");
			}
		}

		private void VisitMethodCallArguments(MethodCallExpression expression)
		{
			for (int i = 0; i < expression.Arguments.Count; i++)
			{
				VisitMethodCallArgument(expression.Arguments[i]);
				if (i < expression.Arguments.Count - 1)
				{
					Builder.Append(", ");
				}
			}
		}

		private void VisitMethodCallArgument(Expression expression)
		{
			// Because arguments to indexers and/or methods are often captured (as in closures) each argument needs special handling...
			object argumentValue = Expression.Lambda(expression).Compile().DynamicInvoke();
			// ...and whatever is resolved by the late-bound call to the 'block' is stringified according to the standard ToString member.
			Builder.Append(argumentValue.ToNullSafeString());
		}
	}
}
