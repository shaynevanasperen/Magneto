using System;

namespace DemoApplication.DomainModel.SingleDatabase
{
	static class SampleDataFactory
	{
		public static TodoItem[] CreateTodoItems()
		{
			var user1 = new User("Bob");
			var user2 = new User("Sue");

			return new[]
			{
				new TodoItem(user1, "1", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user2, "2", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user1, "3", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user2, "4", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user1, "5", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user2, "6", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user1, "7", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user2, "8", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user1, "9", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user2, "10", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(user1, "1", DateTime.Now.AddDays(-1)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-13) },
				new TodoItem(user2, "2", DateTime.Now.AddDays(-1)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-13) },
				new TodoItem(user1, "3", DateTime.Now.AddDays(-2)) { Priority = Priority.Urgent },
				new TodoItem(user2, "4", DateTime.Now.AddDays(-2)) { Priority = Priority.CanWait },
				new TodoItem(user1, "5", DateTime.Now.AddDays(-3)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-11) },
				new TodoItem(user2, "6", DateTime.Now.AddDays(-4)) { Priority = Priority.Urgent, DateCompleted = DateTime.Now.AddDays(-11) },
				new TodoItem(user1, "7", DateTime.Now.AddDays(-5)) { Priority = Priority.CanWait },
				new TodoItem(user2, "8", DateTime.Now.AddDays(-6)) { Priority = Priority.Normal },
				new TodoItem(user1, "9", DateTime.Now.AddDays(-7)) { Priority = Priority.Urgent, DateCompleted = DateTime.Now.AddDays(-9) },
				new TodoItem(user2, "10", DateTime.Now.AddDays(0)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-9) },
				new TodoItem(user1, "11", DateTime.Now.AddDays(1)) { Priority = Priority.Normal },
				new TodoItem(user2, "12", DateTime.Now.AddDays(2)) { Priority = Priority.Urgent },
				new TodoItem(user1, "13", DateTime.Now.AddDays(3)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-7) },
				new TodoItem(user2, "14", DateTime.Now.AddDays(4)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-7) },
				new TodoItem(user1, "15", DateTime.Now.AddDays(5)) { Priority = Priority.Urgent },
				new TodoItem(user2, "16", DateTime.Now.AddDays(6)) { Priority = Priority.CanWait },
				new TodoItem(user1, "17", DateTime.Now.AddDays(7)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-5) },
				new TodoItem(user2, "18", DateTime.Now.AddDays(8)) { Priority = Priority.Urgent, DateCompleted = DateTime.Now.AddDays(-5) },
				new TodoItem(user1, "19", DateTime.Now.AddDays(9)) { Priority = Priority.CanWait },
				new TodoItem(user2, "20", DateTime.Now.AddDays(10)) { Priority = Priority.Normal },
				new TodoItem(user1, "21", DateTime.Now.AddDays(11)) { Priority = Priority.Urgent, DateCompleted = DateTime.Now.AddDays(-3) },
				new TodoItem(user2, "22", DateTime.Now.AddDays(12)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-3) },
				new TodoItem(user1, "23", DateTime.Now.AddDays(13)) { Priority = Priority.Normal },
				new TodoItem(user2, "24", DateTime.Now.AddDays(14)) { Priority = Priority.Urgent },
				new TodoItem(user1, "25", DateTime.Now.AddDays(15)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-1) },
				new TodoItem(user2, "26", DateTime.Now.AddDays(16)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-1) },
				new TodoItem(user1, "27", DateTime.Now.AddDays(17)) { Priority = Priority.Urgent },
				new TodoItem(user2, "28", DateTime.Now.AddDays(18)) { Priority = Priority.CanWait },
				new TodoItem(user1, "29", DateTime.Now.AddDays(19)) { Priority = Priority.Normal },
				new TodoItem(user2, "30", DateTime.Now.AddDays(20)) { Priority = Priority.Urgent },
				new TodoItem(user1, "31", DateTime.Now.AddDays(21)) { Priority = Priority.CanWait },
				new TodoItem(user2, "32", DateTime.Now.AddDays(22)) { Priority = Priority.Normal },
				new TodoItem(user1, "33", DateTime.Now.AddDays(23)) { Priority = Priority.Urgent },
				new TodoItem(user2, "34", DateTime.Now.AddDays(24)) { Priority = Priority.CanWait },
				new TodoItem(user1, "35", DateTime.Now.AddDays(25)) { Priority = Priority.Normal },
				new TodoItem(user2, "36", DateTime.Now.AddDays(26)) { Priority = Priority.Urgent },
				new TodoItem(user1, "37", DateTime.Now.AddDays(27)) { Priority = Priority.CanWait },
				new TodoItem(user2, "38", DateTime.Now.AddDays(28)) { Priority = Priority.Normal },
				new TodoItem(user1, "39", DateTime.Now.AddDays(29)) { Priority = Priority.Urgent },
				new TodoItem(user2, "40", DateTime.Now.AddDays(30)) { Priority = Priority.CanWait },
				new TodoItem(user1, "41", DateTime.Now.AddDays(31)) { Priority = Priority.Normal },
				new TodoItem(user2, "42", DateTime.Now.AddDays(32)) { Priority = Priority.Urgent },
				new TodoItem(user1, "43", DateTime.Now.AddDays(33)) { Priority = Priority.CanWait },
				new TodoItem(user2, "44", DateTime.Now.AddDays(34)) { Priority = Priority.Normal },
				new TodoItem(user1, "45", DateTime.Now.AddDays(35)) { Priority = Priority.Urgent },
				new TodoItem(user2, "46", DateTime.Now.AddDays(36)) { Priority = Priority.CanWait },
				new TodoItem(user1, "47", DateTime.Now.AddDays(37)) { Priority = Priority.Normal },
				new TodoItem(user2, "48", DateTime.Now.AddDays(38)) { Priority = Priority.Urgent },
				new TodoItem(user1, "49", DateTime.Now.AddDays(39)) { Priority = Priority.CanWait },
				new TodoItem(user2, "50", DateTime.Now.AddDays(40)) { Priority = Priority.Normal },
				new TodoItem(user1, "51", DateTime.Now.AddDays(41)) { Priority = Priority.Urgent },
				new TodoItem(user2, "52", DateTime.Now.AddDays(42)) { Priority = Priority.CanWait },
				new TodoItem(user1, "53", DateTime.Now.AddDays(43)) { Priority = Priority.Normal },
				new TodoItem(user2, "54", DateTime.Now.AddDays(44)) { Priority = Priority.Urgent },
				new TodoItem(user1, "55", DateTime.Now.AddDays(45)) { Priority = Priority.CanWait },
				new TodoItem(user2, "56", DateTime.Now.AddDays(46)) { Priority = Priority.Normal },
				new TodoItem(user1, "57", DateTime.Now.AddDays(47)) { Priority = Priority.Urgent },
				new TodoItem(user2, "58", DateTime.Now.AddDays(48)) { Priority = Priority.CanWait },
				new TodoItem(user1, "59", DateTime.Now.AddDays(49)) { Priority = Priority.Normal },
				new TodoItem(user2, "60", DateTime.Now.AddDays(50)) { Priority = Priority.Urgent }
			};
		}
	}
}
