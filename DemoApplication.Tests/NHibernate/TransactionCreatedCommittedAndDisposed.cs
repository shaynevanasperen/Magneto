using Machine.Fakes;
using Machine.Specifications;
using NHibernate;

namespace DemoApplication.Tests.NHibernate
{
	[Behaviors]
	class TransactionCreatedCommittedAndDisposed<TSubject> : WithSubject<TSubject> where TSubject : class
	{
		It should_create_the_transaction = () =>
			The<ISession>().WasToldTo(x => x.BeginTransaction());

		It should_commit_the_transaction = () =>
			The<ITransaction>().WasToldTo(x => x.Commit());

		It should_dispose_the_transaction = () =>
			The<ITransaction>().WasToldTo(x => x.Dispose());
	}
}
