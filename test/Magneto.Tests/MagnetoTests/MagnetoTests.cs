using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace Magneto.Tests.MagnetoTests
{
	public abstract class GettingContext : ScenarioFor<MagnetoTest>
	{
		readonly ServiceProvider _serviceProvider = new ServiceProvider();

		protected Foo Foo;
		protected Bar Bar;
		protected Baz Baz;

		public override void Setup()
		{
			_serviceProvider.Register(new Foo());
			_serviceProvider.Register(new Bar());
			_serviceProvider.Register(new Baz());
			SUT = new MagnetoTest(_serviceProvider);
		}

		protected void ThenTheContextIsResolvedCorrectly()
		{
			_serviceProvider.Services.SingleOrDefault(x => x.Value == Foo).Should().NotBeNull();
			_serviceProvider.Services.SingleOrDefault(x => x.Value == Bar).Should().NotBeNull();
			_serviceProvider.Services.SingleOrDefault(x => x.Value == Baz).Should().NotBeNull();
		}

		public class ForNormalType : GettingContext
		{
			protected void WhenGettingContext()
			{
				Foo = SUT.GetContextPublic<Foo>();
				Bar = SUT.GetContextPublic<Bar>();
				Baz = SUT.GetContextPublic<Baz>();
			}
		}

		public class ForValueTupleType : GettingContext
		{
			protected void WhenGettingContext()
			{
				var context = SUT.GetContextPublic<(Foo, Bar, Baz)>();
				(Foo, Bar, Baz) = context;
			}
		}
	}

	public class ServiceProvider : IServiceProvider
	{
		private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

		public IReadOnlyCollection<KeyValuePair<Type, object>> Services => _services;

		public void Register(object instance) => _services.TryAdd(instance.GetType(), instance);

		public object GetService(Type serviceType) => _services.GetValueOrDefault(serviceType);
	}

	public class MagnetoTest : Magneto
	{
		public MagnetoTest(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public TContext GetContextPublic<TContext>() => GetContext<TContext>();
	}

	public class Foo { }
	public class Bar { }
	public class Baz { }
}
