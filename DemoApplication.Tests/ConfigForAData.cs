using Data.Operations;
using Machine.Fakes;

namespace DemoApplication.Tests
{
	class ConfigForAData<TContext> where TContext : class
	{
		OnEstablish context = fakeAccessor =>
		{
			fakeAccessor.The<IData<TContext>>().WhenToldTo(x => x.Context).Return(fakeAccessor.The<TContext>);
		};
	}
}