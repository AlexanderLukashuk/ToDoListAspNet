using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using ToDoListAspNet.Models;
using ToDoListAspNet.Models.Repo;

namespace ToDoListAspNet.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IToDoRepository _repository;

    public HomeController(ILogger<HomeController> logger, IToDoRepository repository)
    {
        _logger = logger;
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

    public IActionResult Index() => View(_repository.ToDos);

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

