using System;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNetLibrary.Models.Entities;

namespace ToDoListAspNetLibrary.Models.Data
{
	public class ToDoListDBContext : DbContext
	{
		public ToDoListDBContext(DbContextOptions<ToDoListDBContext> options)
			: base(options)
		{
		}

		public DbSet<ToDo> ToDos { get; set; }
	}
}

