using DemoApplication.NHibernate.LazySessions;
using Machine.Fakes;
using NHibernate;

namespace DemoApplication.Tests.NHibernate.LazySessions
{
	class ConfigForATodoItemsContext
	{
		OnEstablish context = fakeAccessor =>
		{
			fakeAccessor.The<ITodoItemsContext>().WhenToldTo(x => x.Session).Return(fakeAccessor.The<ISession>);
		};
	}
}