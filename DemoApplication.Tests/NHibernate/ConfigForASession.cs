using Machine.Fakes;
using NHibernate;

namespace DemoApplication.Tests.NHibernate
{
	class ConfigForASession
	{
		OnEstablish context = fakeAccessor =>
		{
			fakeAccessor.The<ISession>().WhenToldTo(x => x.BeginTransaction()).Return(fakeAccessor.The<ITransaction>);
		};
	}
}