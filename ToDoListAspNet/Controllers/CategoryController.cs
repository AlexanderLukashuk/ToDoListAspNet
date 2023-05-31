using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Entities;
using ToDoListAspNetLibrary.Models.Repo;
using ToDoListAspNetLibrary.Services;
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

        private ToDoService todoService;

        private CategoryService categoryService;

        private ToDoListDBContext _todoContext;

        public CategoryController (ICategoryRepository repository, IToDoRepository toDoRepository, CategoryDBContext context, IConfiguration configuration, ToDoListDBContext todoContext)
        {
            _repository = repository;
            _context = context;
            _toDoRepository = toDoRepository;
            connectionString = configuration.GetConnectionString("ToDoWebsite") ?? throw new InvalidOperationException("Connection string \"ToDoWebsite\" not found.");
            todoService = new ToDoService(connectionString);
            _todoContext = todoContext;
            categoryService = new CategoryService(connectionString);
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

            if (todos.Count > 0)
            {
                var percentagePerToDo = 100 / todos.Count;

                foreach (var t in todos)
                {
                    if (t.Status == ToDo.ToDoStatus.Completed)
                    {
                        category.Progress += Convert.ToInt32(percentagePerToDo);
                    }
                }
            }

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
                categoryService.Create(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            categoryService.Delete(id, _context, todoService, _todoContext);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                categoryService.Update(id, category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }
    }
}

