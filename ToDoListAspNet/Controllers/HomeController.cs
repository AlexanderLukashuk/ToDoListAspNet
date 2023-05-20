using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using ToDoListAspNet.Models;
using ToDoListAspNet.Models.Data;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Repo;

namespace ToDoListAspNet.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IToDoRepository _repository;

    private readonly ToDoListDBContext _context;

    public HomeController(ILogger<HomeController> logger, IToDoRepository repository, ToDoListDBContext context)
    {
        _logger = logger;
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _context = context;
    }

    //public IActionResult Index()
    //{
    //    return View();
    //}

    //[Route("/")]
    //public ActionResult Index()
    //{
    //    //ToDo starterItem = new ToDo("Add first item to To Do List");
    //    ViewBag.ShowControlls = true;
    //    //return View(starterItem);
    //    return View();
    //}

    //public IActionResult Index() => View(_repository.ToDos);

    //[Route("/createToDo")]
    //public IActionResult CreateToDo()
    //{
    //    return View();
    //}

    ////[HttpPost("/todos/create")]
    ////[Route("/create")]
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> CreateToDo(ToDo todo)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        _context.Add(todo);
    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }
    //    return View(todo);
    //}

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

