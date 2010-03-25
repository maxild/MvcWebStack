using System.Collections;
using System.Collections.Generic;
using Maxfire.CommonServiceLocator.CastleAdapter.UnitTests.Components;
using Microsoft.Practices.ServiceLocation;
using Xunit;

namespace Maxfire.CommonServiceLocator.CastleAdapter.UnitTests
{
	public class ServiceLocatorSpecs : IUseFixture<ServiceLocatorFixture>
	{
		private ServiceLocatorFixture _fixture;
		
		public void SetFixture(ServiceLocatorFixture fixture)
		{
			_fixture = fixture;
		}

		public IServiceLocator ServiceLocator 
		{ 
			get { return _fixture.ServiceLocator; }
		}
		
		[Fact]
		public void GetInstance()
		{
			ILogger instance = ServiceLocator.GetInstance<ILogger>();
			Assert.NotNull(instance);
		}

		[Fact]
		public void AskingForInvalidComponentShouldRaiseActivationException()
		{
			Assert.Throws<ActivationException>(() => ServiceLocator.GetInstance<IDictionary>());
		}

		[Fact]
		public void GetNamedInstance()
		{
			ILogger instance = ServiceLocator.GetInstance<ILogger>(typeof(AdvancedLogger).FullName);
			Assert.IsType<AdvancedLogger>(instance);
		}

		[Fact]
		public void GetNamedInstance2()
		{
			ILogger instance = ServiceLocator.GetInstance<ILogger>(typeof(SimpleLogger).FullName);
			Assert.IsType<SimpleLogger>(instance);
		}

		[Fact]
		public void GetNamedInstance_WithEmptyName()
		{
			Assert.Throws<ActivationException>(() => ServiceLocator.GetInstance<ILogger>(""));
		}

		[Fact]
		public void GetUnknownInstance2()
		{
			Assert.Throws<ActivationException>(() => ServiceLocator.GetInstance<ILogger>("test"));
		}

		[Fact]
		public void GetAllInstances()
		{
			IEnumerable<ILogger> instances = ServiceLocator.GetAllInstances<ILogger>();
			IList<ILogger> list = new List<ILogger>(instances);
			Assert.Equal(2, list.Count);
		}

		[Fact]
		public void GetlAllInstance_ForUnknownType_ReturnEmptyEnumerable()
		{
			IEnumerable<IDictionary> instances = ServiceLocator.GetAllInstances<IDictionary>();
			IList<IDictionary> list = new List<IDictionary>(instances);
			Assert.Equal(0, list.Count);
		}

		[Fact]
		public void GenericOverload_GetInstance()
		{
			Assert.Equal(
				ServiceLocator.GetInstance<ILogger>().GetType(),
				ServiceLocator.GetInstance(typeof(ILogger), null).GetType()
				);
		}

		[Fact]
		public void GenericOverload_GetInstance_WithName()
		{
			Assert.Equal(
				ServiceLocator.GetInstance<ILogger>(typeof(AdvancedLogger).FullName).GetType(),
				ServiceLocator.GetInstance(typeof(ILogger), typeof(AdvancedLogger).FullName).GetType()
				);
		}

		[Fact]
		public void Overload_GetInstance_NoName_And_NullName()
		{
			Assert.Equal(
				ServiceLocator.GetInstance<ILogger>().GetType(),
				ServiceLocator.GetInstance<ILogger>(null).GetType()
				);
		}

		[Fact]
		public void GenericOverload_GetAllInstances()
		{
			List<ILogger> genericLoggers = new List<ILogger>(ServiceLocator.GetAllInstances<ILogger>());
			List<object> plainLoggers = new List<object>(ServiceLocator.GetAllInstances(typeof(ILogger)));
			Assert.Equal(genericLoggers.Count, plainLoggers.Count);
			for (int i = 0; i < genericLoggers.Count; i++)
			{
				Assert.Equal(
					genericLoggers[i].GetType(),
					plainLoggers[i].GetType());
			}
		}
	}
}