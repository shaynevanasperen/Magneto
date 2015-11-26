using System.Threading.Tasks;

namespace DemoApplication.DomainModel.SingleDatabase
{
	public interface IUserAlertService
	{
		Task SendAsync(int userId, string message);
	}
}