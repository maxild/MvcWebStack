using System;
using System.Xml;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5.AssertionExtensions
{
	public class ElementListVerifier : FragmentVerifier<ElementListVerifier>
	{
		private readonly XmlNodeList _nodeList;
		private XmlElement _currentElement;
		
		public ElementListVerifier(string xhtml, XmlDocument document) : base(xhtml)
		{
			if (document == null || document.DocumentElement == null)
			{
				throw new NullReferenceException();
			}
			_nodeList = document.DocumentElement.ChildNodes;
		}

		public ElementListVerifier HasCount(int count)
		{
			Assert.Equal(count, _nodeList.Count);
			return this;
		}

		public ElementListVerifier ElementAt(int index)
		{
			_currentElement = _nodeList[index] as XmlElement;
			return this;
		}

		protected override XmlElement CurrentElement
		{
			get { return _currentElement; }
		}
	}
}