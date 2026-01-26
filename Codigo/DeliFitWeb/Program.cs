using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace DeliFitWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        var connectionString = builder.Configuration
            .GetConnectionString("DeliFitConnection")
            ?? throw new InvalidOperationException("Connection string não configurada.");
        IServiceCollection serviceCollection = builder.Services.AddDbContext<DeliFitContext>(options => options.UseMySQL(connectionString));
        builder.Services.AddTransient<IItemService, ItemService>();
        builder.Services.AddTransient<IClienteService, ClienteService>();
        builder.Services.AddTransient<IPedidoService, PedidoService>();
        builder.Services.AddTransient<IRestauranteService, RestauranteService>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
    }
}