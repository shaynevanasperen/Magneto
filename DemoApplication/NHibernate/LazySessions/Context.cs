using NHibernate.Sessions;

namespace DemoApplication.NHibernate.LazySessions
{
	public interface ITodoItemsContext : ISessionContext { }
	public interface IUsersContext : ISessionContext { }

	class TodoItemsContext : SessionContext, ITodoItemsContext
	{
		public TodoItemsContext(ILazySessionScope lazySessionScope) : base(lazySessionScope) { }
	}

	class UsersContext : SessionContext, IUsersContext
	{
		public UsersContext(ILazySessionScope lazySessionScope) : base(lazySessionScope) { }
	}
}
