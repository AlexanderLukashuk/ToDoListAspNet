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
                        ToDoStatus = "not started"
                    },
                    new ToDo
                    {
                        Name = "Lifejacket",
                        Description = "Protective and fashionable",
                        DeadLine = DateTime.Now,
                        ToDoStatus = "not started"
                    },
                    new ToDo
                    {
                        Name = "Soccer Ball",
                        Description = "FIFA-approved size and weight",
                        DeadLine = DateTime.Now,
                        ToDoStatus = "not started"
                    },
                    new ToDo
                    {
                        Name = "Corner Flags",
                        Description = "Give your playing field a professional touch",
                        DeadLine = DateTime.Now,
                        ToDoStatus = "not started"
                    }
                    //new Product
                    //{
                    //    Name = "Stadium",
                    //    Description = "Flat-packed 35,000-seat stadium",
                    //    Category = "Soccer",
                    //    Price = 79500
                    //},
                    //new Product
                    //{
                    //    Name = "Thinking Cap",
                    //    Description = "Improve brain efficiency by 75%",
                    //    Category = "Chess",
                    //    Price = 16
                    //},
                    //new Product
                    //{
                    //    Name = "Unsteady Chair",
                    //    Description = "Secretly give your opponent a disadvantage",
                    //    Category = "Chess",
                    //    Price = 29.95m
                    //},
                    //new Product
                    //{
                    //    Name = "Human Chess Board",
                    //    Description = "A fun game for the family",
                    //    Category = "Chess",
                    //    Price = 75
                    //},
                    //new Product
                    //{
                    //    Name = "Bling-Bling King",
                    //    Description = "Gold-plated, diamond-studded King",
                    //    Category = "Chess",
                    //    Price = 1200
                    //}
                );
                context.SaveChanges();
            }
        }
    }
}

