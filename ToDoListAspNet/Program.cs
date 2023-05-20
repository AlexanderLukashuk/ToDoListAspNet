using Microsoft.EntityFrameworkCore;
using ToDoListAspNet.Models.Data;
using ToDoListAspNet.Models.Repo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoListDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ToDoWebsite")
    ?? throw new InvalidOperationException("Connection string \"ToDoWebsite\" not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IToDoRepository, EFToDoRepository>();

var app = builder.Build();

SeedData.EnsurePopulated(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

app.Run();

