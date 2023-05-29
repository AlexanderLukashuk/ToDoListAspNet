using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;
using ToDoListAspNetLibrary.Services;

namespace ToDoListAspNet.Tests
{
	public class ToDoServiceTests
	{
        private const string connectionString = "Data Source=localhost;Initial Catalog=ToDoListAspNet;User ID=sa;Password=pOStgRES1488;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        [Fact]
        public void Create_ShouldInsertToDoIntoDatabase()
        {
            // Arrange
            string todoName = "Test ToDo";
            string todoDescription = "Test description";
            DateTime? todoDeadline = new DateTime(2023, 5, 22);
            int categoryId = 1;

            var todoService = new ToDoService(connectionString); // Replace with your ToDoService constructor

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                // Create a new ToDo
                var todo = new ToDo
                {
                    Name = todoName,
                    Description = todoDescription,
                    DeadLine = todoDeadline,
                    Status = ToDo.ToDoStatus.NotStarted
                };

                // Call the Create method
                todoService.Create(todo, categoryId);

                // Retrieve the ToDo from the database
                string query = "SELECT * FROM ToDos WHERE Name = @name;";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", todoName);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        // Assert
                        Assert.True(reader.Read());
                        Assert.Equal(todoName, reader.GetString(1));
                        Assert.Equal(todoDescription, reader.GetString(2));
                        Assert.Equal(todoDeadline, reader.GetDateTime(3));
                        Assert.Equal(ToDo.ToDoStatus.NotStarted, (ToDo.ToDoStatus)reader.GetInt32(4));
                        Assert.Equal(categoryId, reader.GetInt32(5));
                    }
                }
            }
        }

        [Fact]
        public void Update_ShouldUpdateToDoInDatabase()
        {
            // Arrange
            int todoId = 5;
            string updatedName = "Updated ToDo";
            string updatedDescription = "Updated description";

            ToDo originalToDo = new ToDo
            {
                Id = 5,
                Name = "Kayak2",
                Description = "A boat for one person",
                DeadLine = new DateTime(2023, 05, 25, 22, 34, 26),
                Status = ToDo.ToDoStatus.NotStarted,
                CategoryId = 1
            };

            var todoService = new ToDoService(connectionString); // Replace with your ToDoService constructor

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                //ToDo originalToDo = todoService.GetToDoById(todoId);
                ToDo updatedToDo = new ToDo
                {
                    Id = originalToDo.Id,
                    Name = updatedName,
                    Description = updatedDescription,
                    DeadLine = originalToDo.DeadLine,
                    CategoryId = originalToDo.CategoryId
                };

                // Call the Update method
                todoService.Update(todoId, updatedToDo);

                // Retrieve the updated ToDo from the database
                string query = "SELECT * FROM ToDos WHERE Id = @id;";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", todoId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        // Assert
                        Assert.True(reader.Read());
                        Assert.Equal(updatedName, reader.GetString(1));
                        Assert.Equal(updatedDescription, reader.GetString(2));
                    }
                }

                //todoService.Update(todoId, originalToDo);
            }
        }

        private DbContextOptions<ToDoListDBContext> GetInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<ToDoListDBContext>()
                .UseInMemoryDatabase("ToDoWebsite")
                .Options;
        }

        private ToDoListDBContext todoContext = new ToDoListDBContext();

        [Fact]
        public void Delete_ShouldDeleteToDoFromDatabase()
        {
            // Arrange
            int todoId = 33;
            string name = "Test delete";
            string descr = "delete todo";
            DateTime deadline = DateTime.Now;
            int categoryId = 1;

            ToDo tempToDo = new ToDo { Name = name, Description = descr, DeadLine = deadline, Status = ToDo.ToDoStatus.NotStarted, CategoryId = categoryId };

            var options = GetInMemoryDatabaseOptions();
            using (todoContext)
            {
                // Add a ToDo to the database for deletion
                todoContext.ToDos.Add(tempToDo);
                todoContext.SaveChanges();
            }

            var todoService = new ToDoService(connectionString);

            // Act
            todoService.Delete(tempToDo.Id, todoContext);

            // Assert
            using (todoContext)
            {
                var deletedToDo = todoContext.ToDos.Find(tempToDo.Id);
                Assert.Null(deletedToDo);
            }
        }
    }
}

