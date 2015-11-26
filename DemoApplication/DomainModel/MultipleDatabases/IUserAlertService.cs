using System;
using System.Threading.Tasks;

namespace DemoApplication.DomainModel.MultipleDatabases
{
	public interface IUserAlertService
	{
		Task SendAsync(Guid userId, string message);
	}
}