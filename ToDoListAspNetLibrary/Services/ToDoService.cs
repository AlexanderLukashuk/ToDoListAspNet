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

		public void Create(ToDo todo, int categoryId)
		{
            try
            {
                using (connection)
                {
                    try
                    {
                        connection.Open();

                        string query =
                            "INSERT INTO ToDos (Name, Description, DeadLine, Status, CategoryId)" +
                            "VALUES (@name, @descr, CAST(@deadLine AS DateTime), @status, @categoryId);";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", todo.Name);
                            command.Parameters.AddWithValue("@descr", string.IsNullOrEmpty(todo.Description) ? "No description" : todo.Description);
                            command.Parameters.AddWithValue("@deadLine", todo.DeadLine.HasValue ? (object)todo.DeadLine : DateTime.MaxValue);
                            command.Parameters.AddWithValue("@status", ToDo.ToDoStatus.NotStarted);
                            command.Parameters.AddWithValue("@categoryId", categoryId);

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

                    // scram.org кокосо троелсон рихтер албахари
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

        public void FinishToDo(int id, ToDoListDBContext context)
        {
            var todo = context.ToDos.Find(id);
            if (todo != null)
            {
                todo.Status = ToDoStatus.Completed;
                context.SaveChanges();
            }
        }

        public IEnumerable<ToDo> GetByCategoryId(int categoryId)
        {
            var todos = new List<ToDo>();

            using (connection)
            {
                connection.Open();

                var query = "SELECT Id, Name, Description, DeadLine, Status, CategoryId FROM ToDos WHERE CategoryId = @CategoryId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var todo = new ToDo
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            DeadLine = reader.GetDateTime(3),
                            Status = (ToDo.ToDoStatus)reader.GetInt32(4),
                            CategoryId = reader.GetInt32(5)
                        };

                        todos.Add(todo);
                    }
                }
            }

            return todos;
        }

        public ToDo GetToDoById(int id)
        {
            ToDo todo = new ToDo();

            try
            {
                using (connection)
                {
                    try
                    {
                        connection.Open();

                        string query = "SELECT * FROM ToDos WHERE Id = @id";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var name = reader.GetString(reader.GetOrdinal("Name"));
                                    var desc = reader.GetString(reader.GetOrdinal("Description"));
                                    var deadline = reader.GetDateTime(reader.GetOrdinal("DeadLine"));
                                    var status = reader.GetInt32(reader.GetOrdinal("Status"));
                                    var categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));

                                    todo.Name = name;
                                    todo.Description = desc;
                                    todo.DeadLine = deadline;
                                    switch (status)
                                    {
                                        case 0:
                                            {
                                                todo.Status = ToDo.ToDoStatus.NotStarted;
                                                break;
                                            }
                                        case 1:
                                            {
                                                todo.Status = ToDo.ToDoStatus.InProgress;
                                                break;
                                            }
                                        case 2:
                                            {
                                                todo.Status = ToDo.ToDoStatus.Completed;
                                                break;
                                            }
                                    }
                                    todo.CategoryId = categoryId;
                                }
                            }
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
                Console.WriteLine("ERROR: Can't connect to server" + ex.Message);
            }

            return todo;
        }

        public void Copy(int id)
        {
            ToDo todo = new ToDo();

            try
            {
                using (connection)
                {
                    try
                    {
                        connection.Open();

                        string query = "SELECT * FROM ToDos WHERE Id = @id";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var name = reader.GetString(reader.GetOrdinal("Name"));
                                    var desc = reader.GetString(reader.GetOrdinal("Description"));
                                    var deadline = reader.GetDateTime(reader.GetOrdinal("DeadLine"));
                                    var status = reader.GetInt32(reader.GetOrdinal("Status"));
                                    var categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));

                                    todo.Name = name;
                                    todo.Description = desc;
                                    todo.DeadLine = deadline;
                                    switch (status)
                                    {
                                        case 0:
                                            {
                                                todo.Status = ToDo.ToDoStatus.NotStarted;
                                                break;
                                            }
                                        case 1:
                                            {
                                                todo.Status = ToDo.ToDoStatus.InProgress;
                                                break;
                                            }
                                        case 2:
                                            {
                                                todo.Status = ToDo.ToDoStatus.Completed;
                                                break;
                                            }
                                    }
                                    todo.CategoryId = categoryId;
                                }
                            }
                        }

                        query =
                            "INSERT INTO ToDos (Name, Description, DeadLine, Status, CategoryId)" +
                            "VALUES (@name, @descr, CAST(@deadLine AS DateTime), @status, @categoryId);";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", todo.Name);
                            command.Parameters.AddWithValue("@descr", string.IsNullOrEmpty(todo.Description) ? "No description" : todo.Description);
                            command.Parameters.AddWithValue("@deadLine", todo.DeadLine.HasValue ? (object)todo.DeadLine : DateTime.MaxValue);
                            command.Parameters.AddWithValue("@status", ToDo.ToDoStatus.NotStarted);
                            command.Parameters.AddWithValue("@categoryId", todo.CategoryId);

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
    }
}

