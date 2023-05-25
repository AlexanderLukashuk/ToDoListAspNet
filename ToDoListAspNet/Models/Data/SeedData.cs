using System;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;

namespace ToDoListAspNet.Models.Data
{
	public static class SeedData
	{
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ToDoListDBContext context = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<ToDoListDBContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (!context.ToDos.Any())
            {
                context.ToDos.AddRange(
                    new ToDo
                    {
                        Name = "Kayak",
                        Description = "A boat for one person",
                        DeadLine = DateTime.Now,
                        Status = ToDo.ToDoStatus.NotStarted,
                        CategoryId = 1
                    },
                    new ToDo
                    {
                        Name = "Lifejacket",
                        Description = "Protective and fashionable",
                        DeadLine = DateTime.Now,
                        Status = ToDo.ToDoStatus.NotStarted,
                        CategoryId = 1
                    },
                    new ToDo
                    {
                        Name = "Soccer Ball",
                        Description = "FIFA-approved size and weight",
                        DeadLine = DateTime.Now,
                        Status = ToDo.ToDoStatus.NotStarted,
                        CategoryId = 1
                    },
                    new ToDo
                    {
                        Name = "Corner Flags",
                        Description = "Give your playing field a professional touch",
                        DeadLine = DateTime.Now,
                        Status = ToDo.ToDoStatus.NotStarted,
                        CategoryId = 1
                    },
                    new ToDo
                    {
                        Name = "New ToDo",
                        Description = "Test",
                        DeadLine = DateTime.Now,
                        Status = ToDo.ToDoStatus.NotStarted,
                        CategoryId = 1
                    }
                );
                context.SaveChanges();
            }

            CategoryDBContext catContext = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<CategoryDBContext>();

            if (catContext.Database.GetPendingMigrations().Any())
            {
                catContext.Database.Migrate();
            }

            if (!catContext.Categories.Any())
            {
                catContext.Categories.AddRange(
                    new Category
                    {
                        Name = "Today",
                        Progress = 0
                    },
                    new Category
                    {
                        Name = "Planned",
                        Progress = 0
                    },
                    new Category
                    {
                        Name = "Personal",
                        Progress = 0
                    }
                );
                catContext.SaveChanges();
            }
        }
    }
}

