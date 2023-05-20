using System;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;

namespace ToDoListAspNetLibrary.Models.Repo
{
	public class EFToDoRepository : IToDoRepository
	{
		private ToDoListDBContext context;

		public EFToDoRepository(ToDoListDBContext ctx)
		{
			context = ctx;
		}

		public IQueryable<ToDo> ToDos => context.ToDos;
	}
}

