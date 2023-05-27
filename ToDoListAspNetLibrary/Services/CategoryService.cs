using System;
using System.Data.SqlClient;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;

namespace ToDoListAspNetLibrary.Services
{
	public class CategoryService
	{
        private string connectionString;

        private SqlConnection connection;

        public CategoryService(string connectionSrt)
		{
			connectionString = connectionSrt;
			connection = new SqlConnection(connectionString);
		}

		public void Create(Category category)
		{
            try
            {
                using (connection)
                {
                    try
                    {
                        connection.Open();

                        string query =
                            "INSERT INTO Categories (Name, Progress)" +
                            "VALUES (@name, @progress);";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", category.Name);
                            command.Parameters.AddWithValue("@progress", 0);

                            command.ExecuteNonQuery();
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("ERROR: Can't connect to server" + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("ERROR: Something went wrong" + ex.Message);
            }
        }

        public void Delete(int id, CategoryDBContext context, ToDoService todoService, ToDoListDBContext todoContext)
        {
            Category? category = context.Categories.Find(id);
            if (category != null)
            {
                var todos = todoService.GetByCategoryId(category.Id);
                foreach (var todo in todos)
                {
                    todoService.Delete(todo.Id, todoContext);
                }

                context.Categories.Remove(category);
                context.SaveChanges();
            }
        }

        public void Update(int id, Category category)
        {
            using (connection)
            {
                connection.Open();

                string query = "UPDATE Categories SET Name = @Name WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@Id", category.Id);

                    command.ExecuteNonQuery();
                }

            }
        }
	}
}

