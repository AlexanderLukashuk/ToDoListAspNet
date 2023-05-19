using System;
using ToDoListAspNet.Models.Data;

namespace ToDoListAspNet.Models.Repo
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

