using System;
namespace ToDoListAspNetLibrary.Models.Entities
{
	public class ToDo
	{
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DeadLine { get; set; }
        public string ToDoStatus { get; set; } = string.Empty;
    }
}

