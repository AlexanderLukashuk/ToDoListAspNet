using System;
namespace ToDoListAspNetLibrary.Models.Entities
{
	public class Category
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public int Progress { get; set; }
    }
}

