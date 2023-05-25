using System;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;

namespace ToDoListAspNetLibrary.Models.Repo
{
	public class EFCategoryRepository : ICategoryRepository
	{
        private CategoryDBContext context;

        public EFCategoryRepository(CategoryDBContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Category> Categories => context.Categories;
    }
}

