using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToDoListAspNet.Models;

namespace ToDoListAspNet.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    //public IActionResult Index()
    //{
    //    return View();
    //}

    [Route("/")]
    public ActionResult Index()
    {
        //ToDo starterItem = new ToDo("Add first item to To Do List");
        ViewBag.ShowControlls = true;
        return View(starterItem);
    }

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

