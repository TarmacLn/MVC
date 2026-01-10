using Microsoft.EntityFrameworkCore;

// Configure app settings from appsettings.json.
var builder = WebApplication.CreateBuilder(args);

// Configure database connection.
builder.Services.AddDbContext<UniversityAppDB>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Check database connection.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<UniversityAppDB>();
        if (context.Database.CanConnect())
        {
            Console.WriteLine("Connection successful");
        }
        else
        {
            Console.WriteLine("Connection failed");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error occurred: {e.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
