using System;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNetLibrary.Models.Entities;

namespace ToDoListAspNetLibrary.Models.Data
{
	public class CategoryDBContext : DbContext
    {
		public CategoryDBContext(DbContextOptions<CategoryDBContext> options)
			: base (options)
		{
		}

        public DbSet<Category> Categories { get; set; }
    }
}

