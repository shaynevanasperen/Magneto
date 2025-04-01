using System;
using System.Collections.Generic;
using FluentAssertions;

namespace Magneto.Tests.ConductorTests;

public abstract class GettingContext : ScenarioFor<ConductorTest>
{
	readonly ServiceProvider _serviceProvider = new();

	protected Foo ResolvedFoo = null!;
	protected Bar ResolvedBar = null!;
	protected Baz ResolvedBaz = null!;

	public override void Setup()
	{
		_serviceProvider.Register(new Foo());
		_serviceProvider.Register(new Bar());
		_serviceProvider.Register(new Baz());
		SUT = new(_serviceProvider);
	}

	public class ForUnavailableType : GettingContext
	{
		Func<Attribute> _invocation = null!;

		void WhenContextIsResolved() => _invocation = SUT.Invoking(x => x.ResolveContext<Attribute>());

		void ThenItThrowsAnExceptionStatingThatTheServiceIsNotRegistered() => _invocation.Should().Throw<InvalidOperationException>()
			.Which.Message.Should().Be("No service for type 'System.Attribute' has been registered.");
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

class ServiceProvider : IServiceProvider
{
	readonly Dictionary<Type, object> _services = new();

	public IReadOnlyDictionary<Type, object> Services => _services;

	public void Register(object instance) => _services.TryAdd(instance.GetType(), instance);

	public object? GetService(Type serviceType) => _services.GetValueOrDefault(serviceType);
}

public class ConductorTest(IServiceProvider serviceProvider) : Conductor(serviceProvider)
{
	public TContext ResolveContext<TContext>() => GetContext<TContext>();
}

public class Foo { }
public class Bar { }
public class Baz { }
