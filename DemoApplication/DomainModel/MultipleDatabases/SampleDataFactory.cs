using System;

namespace DemoApplication.DomainModel.MultipleDatabases
{
	static class SampleDataFactory
	{
		public static Tuple<User, User> CreateUsers()
		{
			return Tuple.Create(new User("Bob"), new User("Sue"));
		}

		public static TodoItem[] CreateTodoItems(Tuple<User, User> users)
		{
			return new[]
			{
				new TodoItem(users.Item1.Id, "1", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item2.Id, "2", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item1.Id, "3", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item2.Id, "4", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item1.Id, "5", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item2.Id, "6", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item1.Id, "7", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item2.Id, "8", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item1.Id, "9", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item2.Id, "10", DateTime.Now.AddYears(-1).AddDays(-1)),
				new TodoItem(users.Item1.Id, "1", DateTime.Now.AddDays(-1)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-13) },
				new TodoItem(users.Item2.Id, "2", DateTime.Now.AddDays(-1)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-13) },
				new TodoItem(users.Item1.Id, "3", DateTime.Now.AddDays(-2)) { Priority = Priority.Urgent },
				new TodoItem(users.Item2.Id, "4", DateTime.Now.AddDays(-2)) { Priority = Priority.CanWait },
				new TodoItem(users.Item1.Id, "5", DateTime.Now.AddDays(-3)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-11) },
				new TodoItem(users.Item2.Id, "6", DateTime.Now.AddDays(-4)) { Priority = Priority.Urgent, DateCompleted = DateTime.Now.AddDays(-11) },
				new TodoItem(users.Item1.Id, "7", DateTime.Now.AddDays(-5)) { Priority = Priority.CanWait },
				new TodoItem(users.Item2.Id, "8", DateTime.Now.AddDays(-6)) { Priority = Priority.Normal },
				new TodoItem(users.Item1.Id, "9", DateTime.Now.AddDays(-7)) { Priority = Priority.Urgent, DateCompleted = DateTime.Now.AddDays(-9) },
				new TodoItem(users.Item2.Id, "10", DateTime.Now.AddDays(0)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-9) },
				new TodoItem(users.Item1.Id, "11", DateTime.Now.AddDays(1)) { Priority = Priority.Normal },
				new TodoItem(users.Item2.Id, "12", DateTime.Now.AddDays(2)) { Priority = Priority.Urgent },
				new TodoItem(users.Item1.Id, "13", DateTime.Now.AddDays(3)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-7) },
				new TodoItem(users.Item2.Id, "14", DateTime.Now.AddDays(4)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-7) },
				new TodoItem(users.Item1.Id, "15", DateTime.Now.AddDays(5)) { Priority = Priority.Urgent },
				new TodoItem(users.Item2.Id, "16", DateTime.Now.AddDays(6)) { Priority = Priority.CanWait },
				new TodoItem(users.Item1.Id, "17", DateTime.Now.AddDays(7)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-5) },
				new TodoItem(users.Item2.Id, "18", DateTime.Now.AddDays(8)) { Priority = Priority.Urgent, DateCompleted = DateTime.Now.AddDays(-5) },
				new TodoItem(users.Item1.Id, "19", DateTime.Now.AddDays(9)) { Priority = Priority.CanWait },
				new TodoItem(users.Item2.Id, "20", DateTime.Now.AddDays(10)) { Priority = Priority.Normal },
				new TodoItem(users.Item1.Id, "21", DateTime.Now.AddDays(11)) { Priority = Priority.Urgent, DateCompleted = DateTime.Now.AddDays(-3) },
				new TodoItem(users.Item2.Id, "22", DateTime.Now.AddDays(12)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-3) },
				new TodoItem(users.Item1.Id, "23", DateTime.Now.AddDays(13)) { Priority = Priority.Normal },
				new TodoItem(users.Item2.Id, "24", DateTime.Now.AddDays(14)) { Priority = Priority.Urgent },
				new TodoItem(users.Item1.Id, "25", DateTime.Now.AddDays(15)) { Priority = Priority.CanWait, DateCompleted = DateTime.Now.AddDays(-1) },
				new TodoItem(users.Item2.Id, "26", DateTime.Now.AddDays(16)) { Priority = Priority.Normal, DateCompleted = DateTime.Now.AddDays(-1) },
				new TodoItem(users.Item1.Id, "27", DateTime.Now.AddDays(17)) { Priority = Priority.Urgent },
				new TodoItem(users.Item2.Id, "28", DateTime.Now.AddDays(18)) { Priority = Priority.CanWait },
				new TodoItem(users.Item1.Id, "29", DateTime.Now.AddDays(19)) { Priority = Priority.Normal },
				new TodoItem(users.Item2.Id, "30", DateTime.Now.AddDays(20)) { Priority = Priority.Urgent },
				new TodoItem(users.Item1.Id, "31", DateTime.Now.AddDays(21)) { Priority = Priority.CanWait },
				new TodoItem(users.Item2.Id, "32", DateTime.Now.AddDays(22)) { Priority = Priority.Normal },
				new TodoItem(users.Item1.Id, "33", DateTime.Now.AddDays(23)) { Priority = Priority.Urgent },
				new TodoItem(users.Item2.Id, "34", DateTime.Now.AddDays(24)) { Priority = Priority.CanWait },
				new TodoItem(users.Item1.Id, "35", DateTime.Now.AddDays(25)) { Priority = Priority.Normal },
				new TodoItem(users.Item2.Id, "36", DateTime.Now.AddDays(26)) { Priority = Priority.Urgent },
				new TodoItem(users.Item1.Id, "37", DateTime.Now.AddDays(27)) { Priority = Priority.CanWait },
				new TodoItem(users.Item2.Id, "38", DateTime.Now.AddDays(28)) { Priority = Priority.Normal },
				new TodoItem(users.Item1.Id, "39", DateTime.Now.AddDays(29)) { Priority = Priority.Urgent },
				new TodoItem(users.Item2.Id, "40", DateTime.Now.AddDays(30)) { Priority = Priority.CanWait },
				new TodoItem(users.Item1.Id, "41", DateTime.Now.AddDays(31)) { Priority = Priority.Normal },
				new TodoItem(users.Item2.Id, "42", DateTime.Now.AddDays(32)) { Priority = Priority.Urgent },
				new TodoItem(users.Item1.Id, "43", DateTime.Now.AddDays(33)) { Priority = Priority.CanWait },
				new TodoItem(users.Item2.Id, "44", DateTime.Now.AddDays(34)) { Priority = Priority.Normal },
				new TodoItem(users.Item1.Id, "45", DateTime.Now.AddDays(35)) { Priority = Priority.Urgent },
				new TodoItem(users.Item2.Id, "46", DateTime.Now.AddDays(36)) { Priority = Priority.CanWait },
				new TodoItem(users.Item1.Id, "47", DateTime.Now.AddDays(37)) { Priority = Priority.Normal },
				new TodoItem(users.Item2.Id, "48", DateTime.Now.AddDays(38)) { Priority = Priority.Urgent },
				new TodoItem(users.Item1.Id, "49", DateTime.Now.AddDays(39)) { Priority = Priority.CanWait },
				new TodoItem(users.Item2.Id, "50", DateTime.Now.AddDays(40)) { Priority = Priority.Normal },
				new TodoItem(users.Item1.Id, "51", DateTime.Now.AddDays(41)) { Priority = Priority.Urgent },
				new TodoItem(users.Item2.Id, "52", DateTime.Now.AddDays(42)) { Priority = Priority.CanWait },
				new TodoItem(users.Item1.Id, "53", DateTime.Now.AddDays(43)) { Priority = Priority.Normal },
				new TodoItem(users.Item2.Id, "54", DateTime.Now.AddDays(44)) { Priority = Priority.Urgent },
				new TodoItem(users.Item1.Id, "55", DateTime.Now.AddDays(45)) { Priority = Priority.CanWait },
				new TodoItem(users.Item2.Id, "56", DateTime.Now.AddDays(46)) { Priority = Priority.Normal },
				new TodoItem(users.Item1.Id, "57", DateTime.Now.AddDays(47)) { Priority = Priority.Urgent },
				new TodoItem(users.Item2.Id, "58", DateTime.Now.AddDays(48)) { Priority = Priority.CanWait },
				new TodoItem(users.Item1.Id, "59", DateTime.Now.AddDays(49)) { Priority = Priority.Normal },
				new TodoItem(users.Item2.Id, "60", DateTime.Now.AddDays(50)) { Priority = Priority.Urgent }
			};
		}
	}
}
