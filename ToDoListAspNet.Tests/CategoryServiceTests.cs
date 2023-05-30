using System;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;
using ToDoListAspNetLibrary.Services;

namespace ToDoListAspNet.Tests
{
	public class CategoryServiceTests
	{
        private string connectionString = "Data Source=localhost;Initial Catalog=ToDoListAspNet;User ID=sa;Password=pOStgRES1488;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private CategoryDBContext categoryContext;

        private ToDoListDBContext todoContext;

        private DbContextOptions<CategoryDBContext> GetInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<CategoryDBContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
        }

        private SqliteConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            using (var context = new CategoryDBContext(
                new DbContextOptionsBuilder<CategoryDBContext>()
                    .UseSqlite(connection)
                    .Options))
            {
                context.Database.EnsureCreated();
            }

            return connection;
        }

        private DbContextOptions<ToDoListDBContext> GetInMemoryDatabaseOptionsToDo()
        {
            return new DbContextOptionsBuilder<ToDoListDBContext>()
                .UseSqlite(CreateInMemoryDatabaseToDo())
                .Options;
        }

        private SqliteConnection CreateInMemoryDatabaseToDo()
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
        public void Create_ShouldInsertCategoryIntoDatabase()
        {
            // Arrange
            string categoryName = "TestCategory";
            int expectedProgress = 0;

            var categoryService = new CategoryService(connectionString); // Replace with your CategoryService constructor

            // Act
            using (var connection = new SqlConnection(connectionString))
            {
                // Create a new category
                var category = new Category { Name = categoryName };

                // Call the Create method
                categoryService.Create(category);

                // Retrieve the category from the database
                string query = "SELECT Name, Progress FROM Categories WHERE Name = @name;";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", categoryName);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        // Assert
                        Assert.True(reader.Read());
                        Assert.Equal(categoryName, reader.GetString(0));
                        Assert.Equal(expectedProgress, reader.GetInt32(1));
                    }
                }
            }
        }

        //[Fact]
        //public void Delete_RemovesCategoryAndAssociatedTodos()
        //{
        //    // Arrange
        //    // Create an instance of the database context
        //    using (categoryContext)
        //    {
        //        // Create an instance of the ToDoListDBContext
        //        //var todoContext = new ToDoListDBContext();

        //        // Add test data to the database
        //        var category = new Category { Id = 1, Name = "Test Category" };
        //        categoryContext.Categories.Add(category);
        //        categoryContext.SaveChanges();

        //        // Create a ToDoService instance
        //        var todoService = new ToDoService(connectionString);
        //        // Add test todos associated with the category
        //        todoService.Create(new ToDo { Id = 1, CategoryId = category.Id }, category.Id);
        //        todoService.Create(new ToDo { Id = 2, CategoryId = category.Id }, category.Id);
        //        todoService.Create(new ToDo { Id = 3, CategoryId = category.Id }, category.Id);

        //        // Act
        //        // Call the Delete method
        //        var categoryService = new CategoryService(connectionString);
        //        categoryService.Delete(category.Id, categoryContext, todoService, todoContext);

        //        // Assert
        //        // Verify that the category and associated todos are deleted
        //        var deletedCategory = categoryContext.Categories.Find(category.Id);
        //        var deletedTodos = todoService.GetByCategoryId(category.Id);

        //        Assert.Null(deletedCategory);
        //        Assert.Empty(deletedTodos);
        //    }
        //}

        [Fact]
        public void Delete_ShouldDeleteCategoryAndAssociatedToDos()
        {
            // Arrange
            int categoryId = 6;
            int todoId1 = 1;
            int todoId2 = 2;

            var category = new Category
            {
                Id = categoryId
            };

            var todo1 = new ToDo
            {
                Id = todoId1,
                CategoryId = categoryId
            };

            var todo2 = new ToDo
            {
                Id = todoId2,
                CategoryId = categoryId
            };

            var options = GetInMemoryDatabaseOptions();
            using (var context = new CategoryDBContext(options))
            {
                // Add test data to the category database
                context.Categories.Add(category);
                context.SaveChanges();
            }

            var todoOptions = GetInMemoryDatabaseOptionsToDo();
            using (var todoContext = new ToDoListDBContext(todoOptions))
            {
                //using (var categoryContext = new CategoryDBContext(options))
                //{
                //    var todoService = new ToDoService(connectionString);
                //    var categoryService = new CategoryService(connectionString);

                //    // Add test data to the ToDo database
                //    todoContext.ToDos.Add(todo1);
                //    todoContext.ToDos.Add(todo2);
                //    todoContext.SaveChanges();
                //}

                todoContext.ToDos.Add(todo1);
                todoContext.ToDos.Add(todo2);
                todoContext.SaveChanges();

                using (var categoryContext = new CategoryDBContext(options))
                {
                    var todoService = new ToDoService(connectionString);
                    var categoryService = new CategoryService(connectionString);

                    // Act
                    categoryService.Delete(categoryId, categoryContext, todoService, todoContext);

                    // Assert
                    Assert.Null(categoryContext.Categories.Find(categoryId));

                    Assert.Null(todoContext.ToDos.Find(todoId1));
                    Assert.Null(todoContext.ToDos.Find(todoId2));
                }
            }
        }

        [Fact]
        public void Update_ShouldUpdateCategoryName()
        {
            // Arrange
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Insert a test category into the database
                var initialCategory = new Category { Name = "Initial Name", Progress = 0 };
                InsertCategory(connection, initialCategory);

                // Create an instance of the CategoryService
                var categoryService = new CategoryService(connectionString);

                // Create an updated category
                var updatedCategory = new Category { Name = "Updated Name" };

                // Act
                categoryService.Update(updatedCategory.Id, updatedCategory);

                // Assert
                var retrievedCategory = RetrieveCategory(connection, updatedCategory.Id);
                Assert.Equal(updatedCategory.Name, retrievedCategory.Name);
            }
        }

        private void InsertCategory(SqlConnection connection, Category category)
        {
            string query = "INSERT INTO Categories (Name) VALUES (@Name)";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", category.Name);

                command.ExecuteNonQuery();
            }
        }

        private Category RetrieveCategory(SqlConnection connection, int categoryId)
        {
            string query = "SELECT Id, Name FROM Categories WHERE Id = @Id";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", categoryId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                    }
                }
            }

            return null;
        }
    }
}

