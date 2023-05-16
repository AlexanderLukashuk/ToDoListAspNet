using System;
using Microsoft.EntityFrameworkCore;

namespace ToDoListAspNet.Models.Data
{
	public class ToDoListDBContext : DbContext
	{
		public ToDoListDBContext(DbContextOptions<ToDoListDBContext> options)
			: base(options)
		{
		}

		public DbSet<ToDoListAspNet.Models.ToDo> ToDos { get; set; } = default!;
	}
}

