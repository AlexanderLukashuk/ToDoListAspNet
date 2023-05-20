using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNet.Models;
using ToDoListAspNet.Models.Data;
using ToDoListAspNetLibrary;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;
using ToDoListAspNetLibrary.Models.Repo;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoListAspNet.Controllers
{
    public class ToDosController : Controller
    {
        // GET: /<controller>/
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly IToDoRepository _repository;

        private readonly ToDoListDBContext _context;

        private string connectionString;

        public ToDosController(IToDoRepository repository, ToDoListDBContext context, IConfiguration configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context;
            connectionString = configuration.GetConnectionString("ToDoWebsite") ?? throw new InvalidOperationException("Connection string \"ToDoWebsite\" not found.");
        }

        public IActionResult Index() => View(_repository.ToDos);

        [Route("/createToDo")]
        public IActionResult CreateToDo()
        {
            return View();
        }

        //[HttpPost("/todos/create")]
        //[Route("/create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateToDo(ToDo todo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }
    }
}

