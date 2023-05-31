using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNet.Models;
using ToDoListAspNet.Models.Data;
using ToDoListAspNetLibrary;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;
using ToDoListAspNetLibrary.Models.Repo;
using ToDoListAspNetLibrary.Services;
using static ToDoListAspNetLibrary.Models.Entities.ToDo;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoListAspNet.Controllers
{
    public class ToDosController : Controller
    {
        private readonly IToDoRepository _repository;

        private CategoryDBContext _categoryContex;

        private ToDoListDBContext _context;

        private string connectionString;

        private ToDoService todoService;

        public ToDosController(IToDoRepository repository, ToDoListDBContext context, IConfiguration configuration, CategoryDBContext categoryContext)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context;
            connectionString = configuration.GetConnectionString("ToDoWebsite") ?? throw new InvalidOperationException("Connection string \"ToDoWebsite\" not found.");
            todoService = new ToDoService(connectionString);
            _categoryContex = categoryContext;
        }

        [Route("/ToDos")]
        public IActionResult Index() => View(_repository.ToDos);

        [HttpGet]
        public IActionResult CreateToDo(int categoryId)
        {
            var category = _categoryContex.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                return NotFound();
            }

            var todo = new ToDo();
            todo.CategoryId = categoryId;

            ViewBag.CategoryId = categoryId; // Pass the categoryId to the view

            return View(todo);
        }

        [HttpPost]
        public ActionResult CreateToDo(ToDo todo)
        {
            if (ModelState.IsValid)
            {
                int categoryId = Convert.ToInt32(Request.Form["CategoryId"]);

                todoService.Create(todo, categoryId);
                return RedirectToAction("Details", "Category", new { id = categoryId });
            }

            return View(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            todoService.Delete(id, _context);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var todo = _context.ToDos.Find(id);

            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ToDo todo)
        {
            if (ModelState.IsValid)
            {
                todoService.Update(id, todo);
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StartToDo(int id)
        {
            todoService.StartToDo(id, _context);

            var todo = _context.ToDos.Find(id);
            if (todo != null)
            {
                int categoryId = todo.CategoryId;
                return RedirectToAction("Details", "Category", new { id = categoryId });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishToDo(int id)
        {
            todoService.FinishToDo(id, _context);

            var todo = _context.ToDos.Find(id);
            if (todo != null)
            {
                int categoryId = todo.CategoryId;
                return RedirectToAction("Details", "Category", new { id = categoryId });
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = _context.ToDos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        [HttpPost]
        public IActionResult Copy(int id)
        {
            int categoryId = Convert.ToInt32(Request.Form["CategoryId"]);
            todoService.Copy(id);

            return RedirectToAction("Details", "Category", new { id = categoryId });
        }

        private bool ToDoExists(int id)
        {
            return (_context.ToDos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

