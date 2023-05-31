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

            var categoryService = new CategoryService(connectionString);

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

