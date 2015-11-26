using Data.Operations;
using Machine.Fakes;
using NHibernate;
using NHibernate.Sessions;

namespace DemoApplication.Tests.NHibernate.LazySessions
{
	class ConfigForAData<TData, TContext>
		where TData : class, IData<TContext>
		where TContext : class, ISessionContext
	{
		OnEstablish context = fakeAccessor =>
		{
			fakeAccessor.The<TContext>().WhenToldTo(x => x.Session).Return(fakeAccessor.The<ISession>);
			fakeAccessor.The<TData>().WhenToldTo(x => x.Context).Return(fakeAccessor.The<TContext>);
		};
	}
}