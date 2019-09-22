using System;
using System.Collections.Generic;
using FluentAssertions;

namespace Magneto.Tests.MagnetoTests
{
	public abstract class GettingContext : ScenarioFor<MagnetoTest>
	{
		readonly ServiceProvider _serviceProvider = new ServiceProvider();

		protected Foo ResolvedFoo;
		protected Bar ResolvedBar;
		protected Baz ResolvedBaz;

		public override void Setup()
		{
			_serviceProvider.Register(new Foo());
			_serviceProvider.Register(new Bar());
			_serviceProvider.Register(new Baz());
			SUT = new MagnetoTest(_serviceProvider);
		}

		public class ForNormalType : GettingContext
		{
			void WhenContextIsResolved()
			{
				ResolvedFoo = SUT.ResolveContext<Foo>();
				ResolvedBar = SUT.ResolveContext<Bar>();
				ResolvedBaz = SUT.ResolveContext<Baz>();
			}

			void ThenContextIsResolvedCorrectly() => AssertContextIsResolvedCorrectly();
		}

		public class ForValueTupleType : GettingContext
		{
			void WhenContextIsResolved()
			{
				var context = SUT.ResolveContext<(Foo, Bar, Baz)>();
				(ResolvedFoo, ResolvedBar, ResolvedBaz) = context;
			}

			void ThenContextIsResolvedCorrectly() => AssertContextIsResolvedCorrectly();
		}

		public class ForNestedValueTupleType : GettingContext
		{
			void WhenContextIsResolved()
			{
				var context = SUT.ResolveContext<(Foo, (Bar, Baz))>();
				(ResolvedFoo, (ResolvedBar, ResolvedBaz)) = context;
			}

			void ThenContextIsResolvedCorrectly() => AssertContextIsResolvedCorrectly();
		}

		protected void AssertContextIsResolvedCorrectly()
		{
			ResolvedFoo.Should().Be(_serviceProvider.Services[typeof(Foo)]);
			ResolvedBar.Should().Be(_serviceProvider.Services[typeof(Bar)]);
			ResolvedBaz.Should().Be(_serviceProvider.Services[typeof(Baz)]);
		}
	}

	public class ServiceProvider : IServiceProvider
	{
		private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

		public IReadOnlyDictionary<Type, object> Services => _services;

		public void Register(object instance) => _services.TryAdd(instance.GetType(), instance);

		public object GetService(Type serviceType) => _services.GetValueOrDefault(serviceType);
	}

	public class MagnetoTest : Magneto
	{
		public MagnetoTest(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public TContext ResolveContext<TContext>() => GetContext<TContext>();
	}

	public class Foo { }
	public class Bar { }
	public class Baz { }
}
