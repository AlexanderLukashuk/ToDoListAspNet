using System;
using System.Data.Entity;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
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

            var todoService = new ToDoService(connectionString);

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

            var todoService = new ToDoService(connectionString);

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
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
            }
        }

        private DbContextOptions<ToDoListDBContext> GetInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<ToDoListDBContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
        }

        private SqliteConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            using (var context = new ToDoListDBContext(
                new DbContextOptionsBuilder<ToDoListDBContext>()
                    .UseSqlite(connection)
                    .Options))
            {
                context.Database.EnsureCreated();
            }

            return connection;
        }

        [Fact]
        public void Delete_ShouldDeleteToDoFromDatabase()
        {
            // Arrange
            int todoId = 1;
            string name = "Test delete";
            string descr = "delete todo";
            DateTime deadline = DateTime.Now;
            int categoryId = 1;

            ToDo tempToDo = new ToDo { Id = todoId, Name = name, Description = descr, DeadLine = deadline, Status = ToDo.ToDoStatus.NotStarted, CategoryId = categoryId };

            var options = GetInMemoryDatabaseOptions();

            using (var context = new ToDoListDBContext(options))
            {
                // Add a ToDo to the database for deletion
                context.ToDos.Add(tempToDo);
                context.SaveChanges();
            }

            var todoService = new ToDoService(connectionString);


            // Assert
            using (var context = new ToDoListDBContext(options))
            {
                // Act
                todoService.Delete(tempToDo.Id, context);

                var deletedToDo = context.ToDos.Find(tempToDo.Id);
                Assert.Null(deletedToDo);
            }
        }

        [Fact]
        public void StartToDo_ShouldUpdateToDoStatusToInProgress()
        {
            // Arrange
            int todoId = 1;

            var options = GetInMemoryDatabaseOptions();
            using (var context = new ToDoListDBContext(options))
            {
                // Add test data to the database
                context.ToDos.Add(new ToDo { Id = todoId, Status = ToDo.ToDoStatus.NotStarted });
                context.SaveChanges();
            }

            using (var context = new ToDoListDBContext(options))
            {
                var todoService = new ToDoService(connectionString);

                // Act
                todoService.StartToDo(todoId, context);

                // Assert
                var updatedToDo = context.ToDos.Find(todoId);
                Assert.Equal(ToDo.ToDoStatus.InProgress, updatedToDo.Status);
            }

            using (var context = new ToDoListDBContext(options))
            {
                var todoService = new ToDoService(connectionString);

                todoService.Delete(todoId, context);
            }
        }

        [Fact]
        public void FinishToDo_ShouldUpdateToDoStatusToCompleted()
        {
            // Arrange
            int todoId = 1;

            var options = GetInMemoryDatabaseOptions();
            using (var context = new ToDoListDBContext(options))
            {
                // Add test data to the database
                context.ToDos.Add(new ToDo { Id = todoId, Status = ToDo.ToDoStatus.InProgress });
                context.SaveChanges();
            }

            using (var context = new ToDoListDBContext(options))
            {
                var todoService = new ToDoService(connectionString);

                // Act
                todoService.FinishToDo(todoId, context);

                // Assert
                var updatedToDo = context.ToDos.Find(todoId);
                Assert.Equal(ToDo.ToDoStatus.Completed, updatedToDo.Status);
            }
        }
    }
}

