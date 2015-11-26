using System;
using System.ComponentModel.DataAnnotations;
using Quarks;

namespace DemoApplication.DomainModel.MultipleDatabases
{
	public class User : IdentityFieldProvider<User, Guid>, Users
	{
		protected User() { }

		public User(string name) : this()
		{
			Id = Guid.NewGuid();
			Name = name;
		}

		[Required]
		public virtual string Name { get; set; }
	}
}
