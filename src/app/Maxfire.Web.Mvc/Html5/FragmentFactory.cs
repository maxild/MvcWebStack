using Castle.DynamicProxy;
using Maxfire.Web.Mvc.Html5.Elements;
using Maxfire.Web.Mvc.Html5.HtmlTokens;
using Maxfire.Web.Mvc.Html5.Mixins;

namespace Maxfire.Web.Mvc.Html5
{
	// Rules:
	// 1) Two mixins cannot implement the same interface
	// 2) Mixin cannot implement the same interface as target
	// 3) Forwarding to mixin implementation is implicit -- if mixin implements
	// an interface, it will be forwarded to that mixin instance

	public static class FragmentFactory
	{
		private static readonly ProxyGenerator _generator = new ProxyGenerator();

		public static IOption NewOption()
		{
			var options = new ProxyGenerationOptions();
			var target = new Element<IOption>(HtmlElement.Option);
			options.AddMixinInstance(new ValueMixin<IOption>(target));
			options.AddMixinInstance(new SelectedMixin<IOption>(target));
			return (IOption)_generator.CreateInterfaceProxyWithTarget(typeof(IOption), target, options);
		}
	}

	public interface IOption : IHtmlFragment<IOption>, ISupportsValue<IOption>, ISupportsSelected<IOption>
	{
	}
}