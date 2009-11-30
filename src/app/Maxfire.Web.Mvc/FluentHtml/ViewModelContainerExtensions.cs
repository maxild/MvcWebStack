using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;
using Maxfire.Web.Mvc.FluentHtml.Elements;
using Maxfire.Web.Mvc.FluentHtml.Extensions;

namespace Maxfire.Web.Mvc.FluentHtml
{
	public static class ViewModelContainerExtensions
	{
		/// <summary>
		/// Returns a name to match the value for the HTML name attribute input elements using the same expression. 
		/// </summary>
		/// <typeparam name="T">The type of the ViewModel.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="expression">Expression indicating the ViewModel member.</param>
		public static string NameFor<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression) where T : class
		{
			return expression.GetNameFor(view).LowerCaseFirstWord();
		}

		/// <summary>
		/// Returns a name to match the value for the HTML id attribute input elements using the same expression. 
		/// </summary>
		/// <typeparam name="T">The type of the ViewModel.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="expression">Expression indicating the ViewModel member.</param>
		public static string IdFor<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression) where T : class
		{
			return expression.GetNameFor(view).FormatAsHtmlId().LowerCaseFirstWord();
		}

		/// <summary>
		/// Get display text for a property of the view model.
		/// </summary>
		public static string LabelTextFor<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression) where T : class
		{
			return expression.GetDisplayName();
		}

		/// <summary>
		/// Generate an HTML input element of type 'text' and set its value from the ViewModel based on the expression provided.
		/// </summary>
		/// <typeparam name="T">The type of the ViewModel.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="expression">Expression indicating the ViewModel member associated with the element.</param>
		public static TextBox TextBox<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression) where T : class
		{
			return new TextBox(expression.GetNameFor(view), expression.GetMemberExpression())
				.ApplyBehaviors(view.Behaviors)
				.Value(expression.GetValueFrom(view.ViewModel));
		}

		public static CheckBox CheckBox<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression) where T : class
		{
			var checkbox = new CheckBox(expression.GetNameFor(view), expression.GetMemberExpression())
				.Value("true")
				.ApplyBehaviors(view.Behaviors);
			
			var val = expression.GetValueFrom(view.ViewModel) as bool?;
			if (val != null)
			{
				checkbox.Checked(val.Value);
			}

			return checkbox;
		}

		/// <summary>
		/// Generate an HTML input element of type 'hidden' and set its value from the ViewModel based on the expression provided.
		/// </summary>
		/// <typeparam name="T">The type of the ViewModel.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="expression">Expression indicating the ViewModel member associated with the element.</param>
		public static Hidden Hidden<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression) where T : class
		{
			return new Hidden(expression.GetNameFor(view), expression.GetMemberExpression())
				.ApplyBehaviors(view.Behaviors)
				.Value(expression.GetValueFrom(view.ViewModel));
		}

		/// <summary>
		/// Generate an HTML select element and set the selected option value from the ViewModel based on the expression provided.
		/// </summary>
		/// <typeparam name="T">The type of the ViewModel.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="expression">Expression indicating the ViewModel member associated with the element.</param>
		public static Select Select<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression) where T : class
		{
			return new Select(expression.GetNameFor(view), expression.GetMemberExpression())
				.ApplyBehaviors(view.Behaviors)
				.Selected(expression.GetValueFrom(view.ViewModel));
		}

		/// <summary>
		/// Generate an HTML select element and set the selected options from the ViewModel based on the expression provided.
		/// </summary>
		/// <typeparam name="T">The type of the ViewModel.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="expression">Expression indicating the ViewModel member associated with the element.</param>
		public static MultiSelect MultiSelect<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression) where T : class
		{
			return new MultiSelect(expression.GetNameFor(view), expression.GetMemberExpression())
				.ApplyBehaviors(view.Behaviors)
				.Selected(expression.GetValueFrom(view.ViewModel) as IEnumerable<string>);
		}

		/// <summary>
		/// If ModelState contains an error for the specified view model member, generate an HTML span element with the 
		/// ModelState error message is the specified message is null.   If no class is specified the class attribute 
		/// of the span element will be 'field-validation-error'.
		/// </summary>
		/// <typeparam name="T">The type of the ViewModel.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="expression">Expression indicating the ViewModel member associated with the element.</param>
		public static ValidationMessage ValidationMessage<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression) where T : class
		{
			return ValidationMessage(view, expression, null)
				.ApplyBehaviors(view.Behaviors);
		}

		/// <summary>
		/// If ModelState contains an error for the specified view model member, generate an HTML span element with the 
		/// specified message as inner text, or with the ModelState error message is the specified message is null.  If no
		/// class is specified the class attribute of the span element will be 'field-validation-error'.
		/// </summary>
		/// <typeparam name="T">The type of the ViewModel.</typeparam>
		/// <param name="view">The view.</param>
		/// <param name="expression">Expression indicating the ViewModel member associated with the element.</param>
		/// <param name="message">The error message.</param>
		public static ValidationMessage ValidationMessage<T>(this IViewModelContainer<T> view, Expression<Func<T, object>> expression, string message) where T : class
		{
			string errorMessage = null;
			var name = expression.GetNameFor(view);
			if (view.ViewModelState.ContainsKey(name))
			{
				var modelState = view.ViewModelState[name];
				if (modelState != null && modelState.Errors != null && modelState.Errors.Count > 0)
				{
					errorMessage = message.IsNotEmpty() ? message : modelState.Errors
					                                                	.Select(modelError => modelError.ErrorMessage)
					                                                	.FirstOrDefault(msg => msg.IsNotEmpty());
				}
			}
			return new ValidationMessage(expression.GetMemberExpression())
				.ApplyBehaviors(view.Behaviors)
				.Value(errorMessage);
		}

		public static string ValidationSummary<T>(this IViewModelContainer<T> view) where T : class
		{
			return view.ValidationSummary(null);
		}

		public static string ValidationSummary<T>(this IViewModelContainer<T> view, string message, params Func<string, string>[] htmlAttributes) where T : class
		{
			if (view.ViewModelState.IsValid)
			{
				return null;
			}

			string messageSpan = string.Empty;
			if (message.IsNotEmpty())
			{
				TagBuilder spanTag = new TagBuilder("span");
				foreach (var func in htmlAttributes)
				{
					spanTag.MergeAttribute(func.Method.GetParameters()[0].Name, func(null));
				}
				spanTag.AddCssClass("validation-summary-errors");
				spanTag.SetInnerText(message);
				messageSpan = spanTag.ToString(TagRenderMode.Normal) + Environment.NewLine;
			}

			StringBuilder htmlSummary = new StringBuilder();
			TagBuilder unorderedList = new TagBuilder("ul");
			unorderedList.AddCssClass("validation-summary-errors");

			foreach (ModelState modelState in view.ViewModelState.Values)
			{
				foreach (ModelError modelError in modelState.Errors)
				{
					string errorText = modelError.ErrorMessage;
					string @class = null;
					if (errorText.IsNotEmpty())
					{
						@class = "validation-error";
					}
					else if (modelError.Exception != null)
					{
						errorText = modelError.Exception.Message;
						@class = "exception-error";
					}
					if (errorText.IsNotEmpty())
					{
						TagBuilder listItem = new TagBuilder("li");
						listItem.SetInnerText(errorText);
						listItem.MergeAttribute("class", @class);
						htmlSummary.AppendLine(listItem.ToString(TagRenderMode.Normal));
					}
				}
			}

			unorderedList.InnerHtml = htmlSummary.ToString();

			return messageSpan + unorderedList.ToString(TagRenderMode.Normal);
		}

		/// <summary>
		/// Generate an HTML input element of type 'submit.'
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="text">Value of the 'value' and 'name' attributes.  Also used to derive the 'id' attribute.</param>
		public static SubmitButton SubmitButton<T>(this IViewModelContainer<T> view, string text) where T : class
		{
			return new SubmitButton(text)
				.ApplyBehaviors(view.Behaviors);
		}

		// Todo: Overvej at skrive hele FluentHtml extension methods om til at benytte resource view, og benyt kun FluentHtml som form helpers
		public static string Method<TEditModel, TInputModel, TId>(this OpinionatedResourceSparkView<TEditModel, TInputModel, TId> resourceView)
			where TEditModel : EditModelFor<TInputModel>
			where TInputModel : class, IEntityViewModel<TId>
		{
			string html = string.Empty;
			if (!resourceView.ViewModel.Input.IsTransient)
			{
				html = new Method().ToString();
			}
			return html;
		}
	}
}