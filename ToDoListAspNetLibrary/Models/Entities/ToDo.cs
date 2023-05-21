using System;
namespace ToDoListAspNetLibrary.Models.Entities
{
	public class ToDo
	{
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DeadLine { get; set; }
        public string? ToDoStatus { get; set; }
    }
}

