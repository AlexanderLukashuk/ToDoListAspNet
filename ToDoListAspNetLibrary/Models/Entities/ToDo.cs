using System;
namespace ToDoListAspNetLibrary.Models.Entities
{
	public class ToDo
	{
        public enum ToDoStatus
        {
            NotStarted,
            InProgress,
            Completed
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DeadLine { get; set; }
        public ToDoStatus Status { get; set; }
    }
}

