using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Repo;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoListAspNet.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repository;

        private readonly IToDoRepository _toDoRepository;

        private CategoryDBContext _context;

        public CategoryController (ICategoryRepository repository, IToDoRepository toDoRepository, CategoryDBContext context)
        {
            _repository = repository;
            _context = context;
            _toDoRepository = toDoRepository;
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
    }
}

