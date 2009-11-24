using System.Linq.Expressions;
using System.Text;

namespace Maxfire.Core.Reflection
{
	public class ExpressionNameVisitor
	{
		protected StringBuilder Builder { get; private set; }

		public string ToString(Expression expression)
		{
			if (expression == null)
			{
				return string.Empty;
			}

			Builder = new StringBuilder();

			try
			{
				visit(expression);
				return Builder.ToString();
			}
			finally
			{
				Builder = null;
			}
		}

		private void visit(Expression expression)
		{
			if (expression is UnaryExpression)
			{
				visit(((UnaryExpression)expression).Operand);
			}
			else if (expression is BinaryExpression && expression.NodeType == ExpressionType.ArrayIndex)
			{
				visitArrayIndex((BinaryExpression)expression);
			}
			else if (expression is ConstantExpression)
			{
				visitConstant((ConstantExpression)expression);
			}
			else if (expression is MemberExpression) {
				visitMemberAccess((MemberExpression)expression);
			}	
			else if (expression is MethodCallExpression)
			{
				visitMethodCall((MethodCallExpression)expression);
			}
			else if (expression is LambdaExpression)
			{
				visitLambda((LambdaExpression)expression);
			}
		}

		private void visitLambda(LambdaExpression expression)
		{
			visit(expression.Body);
		}

		private void visitArrayIndex(BinaryExpression expression)
		{
			visit(expression.Left);
			Builder.Append("[");
			visitMethodCallArgument(expression.Right);
			Builder.Append("]");
		}

		private void visitConstant(ConstantExpression expression)
		{
			var index = Expression.Lambda(expression).Compile().DynamicInvoke();
			Builder.Append(index.ToString());
		}

		private void visitMemberAccess(MemberExpression expression)
		{
			if (expression.Expression != null)
			{
				visit(expression.Expression);
			}
			else
			{
				Builder.Append(expression.Member.DeclaringType.Name);
			}

			if (Builder.Length > 0)
			{
				Builder.Append(".");
			}
			Builder.Append(expression.Member.Name);
		}

		private void visitMethodCall(MethodCallExpression expression)
		{
			visit(expression.Object);

			if (expression.IsIndexerProperty())
			{
				Builder.Append("[");
				visitMethodCallArgument(expression.Arguments[0]);
				Builder.Append("]");
			}
			else
			{
				Builder.Append(expression.Method.Name);
				Builder.Append("(");
				for (int i = 0; i < expression.Arguments.Count; i++)
				{
					visitMethodCallArgument(expression.Arguments[i]);
					if (i < expression.Arguments.Count - 1)
					{
						Builder.Append(", ");
					}
				}
				Builder.Append(")");
			}
		}

		private void visitMethodCallArgument(Expression expression)
		{
			// Because arguments to indexers and/or methods are often captured (as in closures) each argument needs special handling...
			var argumentValue = Expression.Lambda(expression).Compile().DynamicInvoke();
			// ...and whatever is resolved by the late-bound call to the 'block' is stringified according to the standard ToString member.
			Builder.Append(argumentValue.ToString());
		}

		//class BuilderScope : IDisposable
		//{
		//    private readonly ExpressionNameVisitor _visitor;
		//    private readonly StringBuilder _previous;
			
		//    public BuilderScope(ExpressionNameVisitor visitor, StringBuilder builder)
		//    {
		//        _visitor = visitor;
		//        _previous = visitor.Builder;
		//        visitor.Builder = builder;
		//    }

		//    public void Dispose()
		//    {
		//        _visitor.Builder = _previous;
		//    }
		//}
	}
}