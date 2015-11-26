using System;
using System.Threading.Tasks;

namespace DemoApplication.DomainModel
{
	class UserAlertService : SingleDatabase.IUserAlertService, MultipleDatabases.IUserAlertService
	{
		public Task SendAsync(int userId, string message)
		{
			Console.WriteLine("Sending alert message: {{{0}}} to user: {{{1}}}", message, userId);
			return Task.FromResult(0);
		}

		public Task SendAsync(Guid userId, string message)
		{
			Console.WriteLine("Sending alert message: {{{0}}} to user: {{{1}}}", message, userId);
			return Task.FromResult(0);
		}
	}
}