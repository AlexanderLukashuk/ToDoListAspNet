using System;
using System.Data.SqlClient;
using ToDoListAspNetLibrary.Models.Entities;

namespace ToDoListAspNetLibrary.Services
{
	public class ToDoService
	{
		public void Create(ToDo todo, string connectionString)
		{
            try
            {
                //using (var connection = new SqlConnection(connectionString))
                using (var connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        string query =
                            "INSERT INTO ToDos (Name, Description, DeadLine, ToDoStatus)" +
                            "VALUES (@name, @descr, CAST(@deadLine AS DateTime), @status);";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", todo.Name);
                            command.Parameters.AddWithValue("@descr", todo.Description);
                            command.Parameters.AddWithValue("@deadLine", todo.DeadLine);
                            command.Parameters.AddWithValue("@status", todo.ToDoStatus);
                            //command.Parameters.AddWithValue("@deadLine", todo.DeadLine.ToString("yyyy-MM-dd HH:mm:ss"));

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

        public void Update(int id, ToDo todo, string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "UPDATE ToDos SET Name = @name, Description = @descr, " +
                    "DeadLine = @deadLine " +
                    "WHERE Id = @id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", todo.Name);
                        command.Parameters.AddWithValue("@descr", todo.Description);
                        command.Parameters.AddWithValue("@deadLine", todo.DeadLine);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
	}
}

