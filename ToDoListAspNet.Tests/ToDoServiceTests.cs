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

        //private DbContextOptions<ToDoListDBContext> GetInMemoryDatabaseOptions()
        //{
        //    return new DbContextOptionsBuilder<ToDoListDBContext>()
        //        .UseInMemoryDatabase("ToDoWebsite")
        //        .Options;
        //}

        //private ToDoListDBContext todoContext = new ToDoListDBContext();

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

            // Act

            // Assert
            using (var context = new ToDoListDBContext(options))
            {
                todoService.Delete(tempToDo.Id, context);

                var deletedToDo = context.ToDos.Find(tempToDo.Id);
                //ToDo deletedToDo = context.ToDos.OrderBy(t => t.Id).Last();
                Assert.Null(deletedToDo);
                //Assert.Null(context.ToDos.OrderBy(t => t.Id).Last());
            }
        }

        //[Fact]
        //public void GetByCategoryId_ShouldReturnToDosWithMatchingCategoryId()
        //{
        //    // Arrange
        //    int categoryId = 6;

        //    var options = GetInMemoryDatabaseOptions();
        //    using (var context = new ToDoListDBContext(options))
        //    {
        //        // Add test data to the database
        //        context.ToDos.Add(new ToDo { Id = 1, CategoryId = categoryId });
        //        context.ToDos.Add(new ToDo { Id = 2, CategoryId = categoryId + 1 });
        //        context.ToDos.Add(new ToDo { Id = 3, CategoryId = categoryId });
        //        context.SaveChanges();
        //    }

        //    using (var context = new ToDoListDBContext(options))
        //    {
        //        var todoService = new ToDoService(connectionString);

        //        // Act
        //        var result = todoService.GetByCategoryId(categoryId);

        //        // Assert
        //        Assert.Collection(result,
        //            todo => Assert.Equal(1, todo.Id),
        //            todo => Assert.Equal(3, todo.Id));
        //    }
        //}

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

        [Fact]
        public void GetByCategoryId_ShouldReturnToDosWithMatchingCategoryId()
        {
            // Arrange
            int categoryId = 6;

            var options = GetInMemoryDatabaseOptions();
            using (var context = new ToDoListDBContext(options))
            {
                // Add test data to the database
                context.ToDos.Add(new ToDo { Id = 1, Name = "Test ToDo 1", CategoryId = categoryId });
                context.ToDos.Add(new ToDo { Id = 2, Name = "Test ToDo 2", CategoryId = 2 });
                context.ToDos.Add(new ToDo { Id = 3, Name = "Test ToDo 3", CategoryId = categoryId });
                context.SaveChanges();
            }

            using (var context = new ToDoListDBContext(options))
            {
                var todoService = new ToDoService(connectionString);

                // Act
                var todos = todoService.GetByCategoryId(categoryId);

                // Assert
                Assert.Collection(todos,
                    todo => Assert.Equal("Test ToDo 1", todo.Name),
                    todo => Assert.Equal("Test ToDo 3", todo.Name)
                );
            }
        }

        [Fact]
        public void GetToDoById_ShouldReturnToDoWithMatchingId()
        {
            // Arrange
            int todoId = 1;
            var expectedToDo = new ToDo
            {
                Id = todoId,
                Name = "Test ToDo",
                Description = "Test Description",
                DeadLine = new DateTime(2023, 1, 1),
                Status = ToDo.ToDoStatus.InProgress,
                CategoryId = 1
            };

            var options = GetInMemoryDatabaseOptions();
            using (var context = new ToDoListDBContext(options))
            {
                // Add test data to the database
                context.ToDos.Add(expectedToDo);
                context.SaveChanges();
            }

            using (var context = new ToDoListDBContext(options))
            {
                var todoService = new ToDoService(connectionString);

                // Act
                var todo = todoService.GetToDoById(todoId);

                // Assert
                Assert.NotNull(todo);
                Assert.Equal(expectedToDo.Name, todo.Name);
                Assert.Equal(expectedToDo.Description, todo.Description);
                Assert.Equal(expectedToDo.DeadLine, todo.DeadLine);
                Assert.Equal(expectedToDo.Status, todo.Status);
                Assert.Equal(expectedToDo.CategoryId, todo.CategoryId);
            }
        }
    }
}

