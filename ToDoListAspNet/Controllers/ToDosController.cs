﻿using System;
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

        private ToDoListDBContext _context;

        private string connectionString;

        private ToDoService todoService;

        public ToDosController(IToDoRepository repository, ToDoListDBContext context, IConfiguration configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context;
            connectionString = configuration.GetConnectionString("ToDoWebsite") ?? throw new InvalidOperationException("Connection string \"ToDoWebsite\" not found.");
            todoService = new ToDoService(connectionString);
        }

        [Route("/ToDos")]
        public IActionResult Index() => View(_repository.ToDos);

        //[Route("/createToDo")]
        public IActionResult CreateToDo()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateToDo(ToDo todo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.ToDos.Add(todo);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(todo);
        //}

        [HttpPost]
        public ActionResult CreateToDo(ToDo todo)
        {
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    using (var command = connection.CreateCommand())
            //    {
            //        //string deadline = todo.DeadLine.ToString("yyyy-mm-dd hh:mm");
            //        command.CommandText = $"INSERT INTO ToDos (Name, Description, DeadLine, ToDoStatus) VALUES ('{todo.Name}', '{todo.Description}', '{todo.DeadLine:yyyy-mm-dd hh:mm}', '{todo.ToDoStatus}')";
            //        try
            //        {
            //            command.ExecuteNonQuery();
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //        }
            //        connection.Close();
            //    }
            //}

            todoService.Create(todo);
            return RedirectToAction(nameof(Index));
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateToDo(ToDo todo)
        //{
        //    if (!ModelState.IsValid)
        //        return View(todo);

        //    _context.ToDos.Add(todo);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            ToDo? todo = _context.ToDos.Find(id);
            if (todo != null)
            {
                _context.ToDos.Remove(todo);
                _context.SaveChanges();
            }
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
        public async Task<IActionResult> StartToDo(int id)
        {
            var todo = await _context.ToDos.FindAsync(id);
            if (todo != null)
            {
                todo.Status = ToDoStatus.InProgress;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishToDo(int id)
        {
            var todo = await _context.ToDos.FindAsync(id);
            if (todo != null)
            {
                todo.Status = ToDoStatus.Completed;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoExists(int id)
        {
            return (_context.ToDos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

