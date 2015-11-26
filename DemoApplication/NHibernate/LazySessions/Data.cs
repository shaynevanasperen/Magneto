using Data.Operations;

namespace DemoApplication.NHibernate.LazySessions
{
	public interface ITodoItemsData : IData<ITodoItemsContext> { }
	public interface IUsersData : IData<IUsersContext> { }

	class TodoItemsData : Data<ITodoItemsContext>, ITodoItemsData
	{
		public TodoItemsData(IData data, ITodoItemsContext context) : base(data, context) { }
	}

	class UsersData : Data<IUsersContext>, IUsersData
	{
		public UsersData(IData data, IUsersContext context) : base(data, context) { }
	}
}
