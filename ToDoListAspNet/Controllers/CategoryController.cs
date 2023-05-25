using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToDoListAspNetLibrary.Models.Repo;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoListAspNet.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repository;

        public CategoryController (ICategoryRepository repository)
        {
            _repository = repository;
        }

        [Route("/Category")]
        public IActionResult Index() => View(_repository.Categories);
    }
}

