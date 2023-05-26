using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;
using ToDoListAspNetLibrary.Models.Repo;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoListAspNet.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repository;

        private readonly IToDoRepository _toDoRepository;

        private CategoryDBContext _context;

        private string connectionString;

        public CategoryController (ICategoryRepository repository, IToDoRepository toDoRepository, CategoryDBContext context, IConfiguration configuration)
        {
            _repository = repository;
            _context = context;
            _toDoRepository = toDoRepository;
            connectionString = configuration.GetConnectionString("ToDoWebsite") ?? throw new InvalidOperationException("Connection string \"ToDoWebsite\" not found.");
        }

        [Route("/Category")]
        public IActionResult Index() => View(_repository.Categories);

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            //var category = _context.Categories.Include(c => c.Todos).FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            var todos = _toDoRepository.ToDos.Where(t => t.CategoryId == category.Id).ToList();
            ViewData["Todos"] = todos;

            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
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
                return RedirectToAction("Index", "Todos"); // Redirect to the main page
            }

            return View(category);
        }
    }
}

