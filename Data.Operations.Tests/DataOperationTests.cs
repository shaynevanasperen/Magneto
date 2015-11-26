using System;
using System.Collections.Generic;
using Machine.Fakes;
using Machine.Specifications;

namespace Data.Operations.Tests
{
	[Subject(typeof(DataOperation))]
	class When_checking_equality_of_two_data_operations_which_have_no_public_properties : WithSubject<TestDataOperation>
	{
		It should_return_true = () =>
			equalsResult.ShouldBeTrue();

		It should_return_the_same_hash_code = () =>
			hashcodeResult.ShouldBeTrue();

		Because of = () =>
		{
			equalsResult = Subject.Equals(other);
			hashcodeResult = Subject.GetHashCode() == other.GetHashCode();
		};

		static TestDataOperation other = new TestDataOperation();
		static bool equalsResult;
		static bool hashcodeResult;
	}

	[Subject(typeof(DataOperation))]
	class When_checking_equality_of_two_data_operations_which_have_all_public_properties_equal_to_each_other : WithSubject<TestDataOperation1>
	{
		It should_return_true = () =>
			equalsResult.ShouldBeTrue();

		It should_return_the_same_hash_code = () =>
			hashcodeResult.ShouldBeTrue();

		Because of = () =>
		{
			equalsResult = Subject.Equals(other);
			hashcodeResult = Subject.GetHashCode() == other.GetHashCode();
		};

		Establish context = () =>
		{
			Subject.Integer = other.Integer = 1;
			Subject.Boolean = other.Boolean = true;
			Subject.Enum = other.Enum = DayOfWeek.Friday;
			Subject.String = other.String = "string";
			Subject.Object = other.Object = new object();
			var item = new object();
			Subject.Collection = new[] { item };
			other.Collection = new[] { item };
		};

		static readonly TestDataOperation1 other = new TestDataOperation1();
		static bool equalsResult;
		static bool hashcodeResult;
	}

	[Subject(typeof(DataOperation))]
	class When_checking_equality_of_two_data_operations_which_dont_have_all_public_properties_equal_to_each_other : WithSubject<TestDataOperation1>
	{
		It should_return_false = () =>
			equalsResult.ShouldBeFalse();

		It should_not_return_the_same_hash_code = () =>
			hashcodeResult.ShouldBeFalse();

		Because of = () =>
		{
			equalsResult = Subject.Equals(other);
			hashcodeResult = Subject.GetHashCode() == other.GetHashCode();
		};

		Establish context = () =>
		{
			other.Integer = 2;
			Subject.Integer = 1;
			Subject.Boolean = other.Boolean = true;
			Subject.Enum = other.Enum = DayOfWeek.Friday;
			Subject.String = other.String = "string";
			Subject.Object = other.Object = new object();
		};

		static readonly TestDataOperation1 other = new TestDataOperation1();
		static bool equalsResult;
		static bool hashcodeResult;
	}

	[Subject(typeof(DataOperation))]
	class When_checking_equality_of_two_data_operations_of_different_types_which_happen_to_have_all_public_properties_equal_to_each_other : WithSubject<TestDataOperation1>
	{
		It should_return_false = () =>
			equalsResult.ShouldBeFalse();

		It should_not_return_the_same_hash_code = () =>
			hashcodeResult.ShouldBeFalse();

		Because of = () =>
		{
			equalsResult = Subject.Equals(other);
			hashcodeResult = Subject.GetHashCode() == other.GetHashCode();
		};

		Establish context = () =>
		{
			Subject.Integer = other.Integer = 1;
			Subject.Boolean = other.Boolean = true;
			Subject.Enum = other.Enum = DayOfWeek.Friday;
			Subject.String = other.String = "string";
			Subject.Object = other.Object = new object();
		};

		static readonly TestDataOperation2 other = new TestDataOperation2();
		static bool equalsResult;
		static bool hashcodeResult;
	}

	[Subject(typeof(DataOperation))]
	class When_checking_equality_of_a_data_operation_against_itself : WithSubject<TestDataOperation1>
	{
		It should_return_true = () =>
			result.ShouldBeTrue();

		Because of = () =>
			result = Subject.Equals(Subject);

		static bool result;
	}

	class TestDataOperation : DataOperation { }

	class TestDataOperation1 : DataOperation
	{
		public int Integer { get; set; }
		public bool Boolean { get; set; }
		public string String { get; set; }
		public DayOfWeek Enum { get; set; }
		public object Object { get; set; }
		public IEnumerable<object> Collection { get; set; }
	}

	class TestDataOperation2 : DataOperation
	{
		public int Integer { get; set; }
		public bool Boolean { get; set; }
		public string String { get; set; }
		public DayOfWeek Enum { get; set; }
		public object Object { get; set; }
	}
}
