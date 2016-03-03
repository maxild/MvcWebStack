using System;
using System.Collections.Generic;
using System.Xml;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5.AssertionExtensions
{
	public class ElementListVerifier : FragmentVerifier<ElementListVerifier>
	{
		class State
		{
			public XmlNodeList Nodes { get; set; }
			public int Index { get; set; }
		}

		private Stack<State> _stack = new Stack<State>();

		public ElementListVerifier(string xhtml, XmlDocument document) : base(xhtml)
		{
			if (document == null || document.DocumentElement == null)
			{
				throw new NullReferenceException();
			}
			_stack.Push(new State {Nodes = document.DocumentElement.ChildNodes});
		}

		public ElementListVerifier(string xhtml, XmlNodeList childNodes)
			: base(xhtml)
		{
			if (childNodes == null)
			{
				throw new NullReferenceException();
			}
			_stack.Push(new State { Nodes = childNodes });
		}

		private State Current
		{
			get { return _stack.Peek(); }
		}

		public ElementListVerifier HasCount(int count)
		{
			Assert.Equal(count, Current.Nodes.Count);
			return this;
		}

		public ElementListVerifier ElementAt(int index)
		{
			Current.Index = index;
			return this;
		}

		public ElementListVerifier GoToChildNodes()
		{
			_stack.Push(new State { Nodes = CurrentElement.ChildNodes });
			return this;
		}

		public ElementListVerifier GoToParentNodes()
		{
			_stack.Pop();
			return this;
		}

		protected override XmlElement CurrentElement
		{
			get { return Current.Nodes[Current.Index] as XmlElement; }
		}
	}
}