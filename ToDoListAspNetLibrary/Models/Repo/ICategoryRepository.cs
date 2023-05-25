using System;
using ToDoListAspNetLibrary.Models.Entities;

namespace ToDoListAspNetLibrary.Models.Repo
{
	public interface ICategoryRepository
	{
        IQueryable<Category> Categories { get; }
    }
}

