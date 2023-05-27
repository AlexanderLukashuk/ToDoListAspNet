using Microsoft.EntityFrameworkCore;
using ToDoListAspNet.Models.Data;
using ToDoListAspNetLibrary.Models.Data;
using ToDoListAspNetLibrary.Models.Repo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoListDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ToDoWebsite")
    ?? throw new InvalidOperationException("Connection string \"ToDoWebsite\" not found."),
    optionsBuilder => optionsBuilder.MigrationsAssembly("ToDoListAspNet")));

builder.Services.AddDbContext<CategoryDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ToDoWebsite")
    ?? throw new InvalidOperationException("Connection string \"ToDoWebsite\" not found."),
    optionsBuilder => optionsBuilder.MigrationsAssembly("ToDoListAspNet")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IToDoRepository, EFToDoRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

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

app.MapGet("/", (HttpContext context) =>
{
    context.Response.Redirect("/Category");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

