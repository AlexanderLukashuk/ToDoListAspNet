using System;
using ToDoListAspNetLibrary.Models.Entities;

namespace ToDoListAspNetLibrary.Models.Repo
{
	public interface IToDoRepository
	{
		IQueryable<ToDo> ToDos { get; }
	}
}

