using Machine.Fakes;
using NHibernate;
using NHibernate.Sessions;

namespace DemoApplication.Tests.NHibernate.LazySession
{
	class ConfigForALazySession
	{
		OnEstablish context = fakeAccessor =>
		{
			fakeAccessor.The<ILazySession>().WhenToldTo(x => x.Current).Return(fakeAccessor.The<ISession>);
		};
	}
}