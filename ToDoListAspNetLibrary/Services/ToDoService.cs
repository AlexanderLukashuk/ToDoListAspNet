using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;
using static ToDoListAspNetLibrary.Models.Entities.ToDo;

namespace ToDoListAspNetLibrary.Services
{
	public class ToDoService
	{
        private string connectionString;

        private SqlConnection connection;

        public ToDoService(string conStr)
        {
            connectionString = conStr;
            connection = new SqlConnection(connectionString);
        }

		public void Create(ToDo todo)
		{
            try
            {
                using (connection)
                {
                    try
                    {
                        connection.Open();

                        string query =
                            "INSERT INTO ToDos (Name, Description, DeadLine, Status)" +
                            "VALUES (@name, @descr, CAST(@deadLine AS DateTime), @status);";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", todo.Name);
                            command.Parameters.AddWithValue("@descr", todo.Description);
                            command.Parameters.AddWithValue("@deadLine", todo.DeadLine);
                            command.Parameters.AddWithValue("@status", ToDo.ToDoStatus.NotStarted);
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

        public void Update(int id, ToDo todo)
        {
            using (connection)
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

        public void Delete(int id, ToDoListDBContext context)
        {
            ToDo? todo = context.ToDos.Find(id);
            if (todo != null)
            {
                context.ToDos.Remove(todo);
                context.SaveChanges();
            }
        }

        public void StartToDo(int id, ToDoListDBContext context)
        {
            var todo = context.ToDos.Find(id);
            if (todo != null)
            {
                todo.Status = ToDoStatus.InProgress;
                context.SaveChanges();
            }
        }
	}
}

