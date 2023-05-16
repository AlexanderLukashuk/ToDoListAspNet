using System;
namespace ToDoListAspNet.Models.Repo
{
	public interface IToDoRepository
	{
		IQueryable<ToDo> ToDos { get; }
	}
}

