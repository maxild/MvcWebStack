﻿using System;
using System.Xml;

namespace Maxfire.Web.Mvc.UnitTests.Html5.AssertionExtensions
{
	public class ElementVerifier : FragmentVerifier<ElementVerifier>
	{
		private readonly XmlElement _rootElement;
		private XmlElement _currentElement;
		
		public ElementVerifier(string xhtml, XmlDocument document) : base(xhtml)
		{
			if (document == null || document.DocumentElement == null)
			{
				throw new NullReferenceException();
			}
			_rootElement = document.DocumentElement;
			_currentElement = _rootElement;
		}

		public ElementVerifier Element(string elementName)
		{
			_currentElement = _rootElement.SelectSingleNode(elementName) as XmlElement;
			return this;
		}

		protected override XmlElement CurrentElement
		{
			get { return _currentElement; }
		}

		//public ElementListVerifier Elements(string xpath)
		//{
		//	XmlNodeList nodes = _rootElement.SelectNodes(xpath);
		//	return new ElementListVerifier(nodes.ToString(), nodes);
		//}
	}
}