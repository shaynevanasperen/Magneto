using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magneto.Core;

namespace Magneto.Tests.Core.OperationTests
{
	public abstract class CheckingEquality : ScenarioFor<Operation>
	{
		protected object Object;

		bool _objectsAreEqual;
		bool _operationsAreEqual;
		bool _hashCodesAreEqual;

		protected void WhenCheckingEquality()
		{
			_objectsAreEqual = SUT.Equals(Object);
			_operationsAreEqual = SUT.Equals((Operation)Object);
			_hashCodesAreEqual = SUT.GetHashCode() == Object?.GetHashCode();
		}

		protected void AssertEqual()
		{
			_objectsAreEqual.Should().BeTrue();
			_operationsAreEqual.Should().BeTrue();
			_hashCodesAreEqual.Should().BeTrue();
		}

		protected void AssertNotEqual()
		{
			_objectsAreEqual.Should().BeFalse();
			_operationsAreEqual.Should().BeFalse();
			_hashCodesAreEqual.Should().BeFalse();
		}

		public class WithNull : CheckingEquality
		{
			void GivenAOperationAndANullReference()
			{
				SUT = new OperationWithoutProperties1();
				Object = null;
			}
			void ThenTheyAreNotEqual() => AssertNotEqual();
		}

		public class WithItself : CheckingEquality
		{
			void GivenTwoReferencesToTheSameOperation()
			{
				SUT = new OperationWithoutProperties1();
				Object = SUT;
			}
			void ThenTheyAreEqual() => AssertEqual();
		}

		public abstract class SameType : CheckingEquality
		{
			public class WithoutProperties : SameType
			{
				void GivenTwoOperationsOfTheSameTypeWithoutProperties()
				{
					SUT = new OperationWithoutProperties1();
					Object = new OperationWithoutProperties1();
				}

				protected void ThenTheyAreEqual() => AssertEqual();

				public class SyncQuery : WithoutProperties
				{
					void GivenTwoSyncQueriesOfTheSameTypeWithoutProperties()
					{
						SUT = new SyncQueryWithoutProperties();
						Object = new SyncQueryWithoutProperties();
					}
				}

				public class AsyncQuery : WithoutProperties
				{
					void GivenTwoAsyncQueriesOfTheSameTypeWithoutProperties()
					{
						SUT = new AsyncQueryWithoutProperties();
						Object = new AsyncQueryWithoutProperties();
					}
				}

				public class SyncCachedQuery : WithoutProperties
				{
					void GivenTwoSyncCachedQueriesOfTheSameTypeWithoutProperties()
					{
						SUT = new SyncCachedQueryWithoutProperties();
						Object = new SyncCachedQueryWithoutProperties();
					}
				}

				public class AsyncCachedQuery : WithoutProperties
				{
					void GivenTwoAsyncCachedQueriesOfTheSameTypeWithoutProperties()
					{
						SUT = new AsyncCachedQueryWithoutProperties();
						Object = new AsyncCachedQueryWithoutProperties();
					}
				}

				public class SyncTransformedCachedQuery : WithoutProperties
				{
					void GivenTwoSyncCachedQueriesOfTheSameTypeWithoutProperties()
					{
						SUT = new SyncTransformedCachedQueryWithoutProperties();
						Object = new SyncTransformedCachedQueryWithoutProperties();
					}
				}

				public class AsyncTransformedCachedQuery : WithoutProperties
				{
					void GivenTwoAsyncTransformedCachedQueriesOfTheSameTypeWithoutProperties()
					{
						SUT = new AsyncTransformedCachedQueryWithoutProperties();
						Object = new AsyncTransformedCachedQueryWithoutProperties();
					}
				}

				public class SyncCommand : WithoutProperties
				{
					void GivenTwoSyncCommandsOfTheSameTypeWithoutProperties()
					{
						SUT = new SyncCommandWithoutProperties();
						Object = new SyncCommandWithoutProperties();
					}
				}

				public class AsyncCommand : WithoutProperties
				{
					void GivenTwoAsyncCommandsOfTheSameTypeWithoutProperties()
					{
						SUT = new AsyncCommandWithoutProperties();
						Object = new AsyncCommandWithoutProperties();
					}
				}

				public class SyncReturningCommand : WithoutProperties
				{
					void GivenTwoSyncCommandsOfTheSameTypeWithoutProperties()
					{
						SUT = new SyncReturningCommandWithoutProperties();
						Object = new SyncReturningCommandWithoutProperties();
					}
				}

				public class AsyncReturningCommand : WithoutProperties
				{
					void GivenTwoAsyncCommandsOfTheSameTypeWithoutProperties()
					{
						SUT = new AsyncReturningCommandWithoutProperties();
						Object = new AsyncReturningCommandWithoutProperties();
					}
				}
			}

			public abstract class WithProperties : SameType
			{
				public class PropertiesEqual : WithProperties
				{
					void GivenTwoOperationsOfTheSameTypeWithPropertiesThatAreEqual()
					{
						var item = new object();
						SUT = new OperationWithProperties1
						{
							Integer = 1,
							Boolean = true,
							String = "String",
							Enum = UriKind.Absolute,
							Object = item,
							Collection = new[] { item }
						};
						Object = new OperationWithProperties1
						{
							Integer = 1,
							Boolean = true,
							String = "String",
							Enum = UriKind.Absolute,
							Object = item,
							Collection = new[] { item }
						};
					}
					void ThenTheyAreEqual() => AssertEqual();
				}

				public class PropertiesNotEqual : WithProperties
				{
					void GivenTwoOperationsOfTheSameTypeWithPropertiesThatAreNotEqual()
					{
						var item = new object();
						SUT = new OperationWithProperties1
						{
							Integer = 1,
							Boolean = true,
							String = "String",
							Enum = UriKind.Absolute,
							Object = item,
							Collection = new[] { item }
						};
						Object = new OperationWithProperties1
						{
							Integer = 2,
							Boolean = true,
							String = "String",
							Enum = UriKind.Absolute,
							Object = item,
							Collection = new[] { item }
						};
					}
					void ThenTheyAreNotEqual() => AssertNotEqual();
				}
			}
		}

		public abstract class DifferentType : CheckingEquality
		{
			public class WithoutProperties : DifferentType
			{
				void GivenTwoOperationsOfDifferentTypeWithoutProperties()
				{
					SUT = new OperationWithoutProperties1();
					Object = new OperationWithoutProperties2();
				}
				void ThenTheyAreNotEqual() => AssertNotEqual();
			}

			public abstract class WithProperties : DifferentType
			{
				public class PropertiesEqual : WithProperties
				{
					void GivenTwoOperationsOfDifferentTypeWithPropertiesThatAreEqual()
					{
						var item = new object();
						SUT = new OperationWithProperties1
						{
							Integer = 1,
							Boolean = true,
							String = "String",
							Enum = UriKind.Absolute,
							Object = item,
							Collection = new[] { item }
						};
						Object = new OperationWithProperties2
						{
							Integer = 1,
							Boolean = true,
							String = "String",
							Enum = UriKind.Absolute,
							Object = item,
							Collection = new[] { item }
						};
					}
					void ThenTheyAreNotEqual() => AssertNotEqual();
				}

				public class PropertiesNotEqual : WithProperties
				{
					void GivenTwoOperationsOfDifferentTypeWithPropertiesThatAreNotEqual()
					{
						var item = new object();
						SUT = new OperationWithProperties1
						{
							Integer = 1,
							Boolean = true,
							String = "String",
							Enum = UriKind.Absolute,
							Object = item,
							Collection = new[] { item }
						};
						Object = new OperationWithProperties2
						{
							Integer = 2,
							Boolean = true,
							String = "String",
							Enum = UriKind.Absolute,
							Object = item,
							Collection = new[] { item }
						};
					}
					void ThenTheyAreNotEqual() => AssertNotEqual();
				}
			}
		}
	}

	public class OperationWithoutProperties1 : Operation { }

	public class OperationWithoutProperties2 : Operation { }

	public class SyncQueryWithoutProperties : SyncQuery<object, object>
	{
		protected override object Query(object context) => throw new NotImplementedException();
	}

	public class AsyncQueryWithoutProperties : AsyncQuery<object, object>
	{
		protected override Task<object> Query(object context, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	}

	public class SyncCachedQueryWithoutProperties : SyncCachedQuery<object, object, object>
	{
		protected override object CacheEntryOptions(object context) => throw new NotImplementedException();
		protected override object Query(object context) => throw new NotImplementedException();
	}

	public class AsyncCachedQueryWithoutProperties : AsyncCachedQuery<object, object, object>
	{
		protected override object CacheEntryOptions(object context) => throw new NotImplementedException();
		protected override Task<object> Query(object context, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	}

	public class SyncTransformedCachedQueryWithoutProperties : SyncTransformedCachedQuery<object, object, object, object>
	{
		protected override object CacheEntryOptions(object context) => throw new NotImplementedException();
		protected override object Query(object context) => throw new NotImplementedException();
		protected override object TransformCachedResult(object cachedResult) => throw new NotImplementedException();
	}

	public class AsyncTransformedCachedQueryWithoutProperties : AsyncTransformedCachedQuery<object, object, object, object>
	{
		protected override object CacheEntryOptions(object context) => throw new NotImplementedException();
		protected override Task<object> Query(object context, CancellationToken cancellationToken = default) => throw new NotImplementedException();
		protected override Task<object> TransformCachedResult(object cachedResult, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	}

	public class SyncCommandWithoutProperties : SyncCommand<object>
	{
		public override void Execute(object context)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncCommandWithoutProperties : AsyncCommand<object>
	{
		public override Task Execute(object context, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	}

	public class SyncReturningCommandWithoutProperties : SyncCommand<object, object>
	{
		public override object Execute(object context) => throw new NotImplementedException();
	}

	public class AsyncReturningCommandWithoutProperties : AsyncCommand<object, object>
	{
		public override Task<object> Execute(object context, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	}

	public class OperationWithProperties1 : Operation
	{
		public int Integer { get; set; }
		public bool Boolean { get; set; }
		public string String { get; set; }
		public UriKind Enum { get; set; }
		public object Object { get; set; }
		public IEnumerable<object> Collection { get; set; }
	}

	public class OperationWithProperties2 : Operation
	{
		public int Integer { get; set; }
		public bool Boolean { get; set; }
		public string String { get; set; }
		public UriKind Enum { get; set; }
		public object Object { get; set; }
		public IEnumerable<object> Collection { get; set; }
	}
}
