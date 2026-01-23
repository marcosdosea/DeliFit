using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

<<<<<<< HEAD
namespace DeliFitWeb
=======
namespace DeliFitWeb;

public class Program
>>>>>>> 2d5f6fbb724caabff9d32faf0eed3a2bb9171e6e
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        var connectionString = builder.Configuration.GetConnectionString("DeliFitConnection");
        builder.Services.AddDbContext<DeliFitContext>(options => options.UseMySQL(connectionString));
        builder.Services.AddTransient<IItemService, ItemService>();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
<<<<<<< HEAD
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString = builder.Configuration.GetConnectionString("DeliFitConnection");
            builder.Services.AddDbContext<DeliFitContext>(options => options.UseMySQL(connectionString));
            builder.Services.AddTransient<IClienteService, ClienteService>();

            var app = builder.Build();

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
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
=======
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
>>>>>>> 2d5f6fbb724caabff9d32faf0eed3a2bb9171e6e
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
