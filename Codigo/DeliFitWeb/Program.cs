using BibliotecaWeb.Helpers;
using Core;
using Core.Service;
using Core.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Service;

namespace DeliFitWeb;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        //var connectionString = builder.Configuration
        //    .GetConnectionString("DeliFitConnection")
        //    ?? throw new InvalidOperationException("Connection string não configurada.");
        //IServiceCollection serviceCollection = builder.Services.AddDbContext<DeliFitContext>(options => options.UseMySQL(connectionString));


        builder.Services.AddDbContext<DeliFitContext>(options =>
            options.UseMySQL(builder.Configuration.GetConnectionString("DeliFitConnection")));

        builder.Services.AddDbContext<IdentityContext>(options =>
            options.UseMySQL(builder.Configuration.GetConnectionString("IdentityConnection")));

        builder.Services.AddDefaultIdentity<UsuarioIdentity>(options =>
        { 
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;

            // Default User settings.
            options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            //options.User.RequireUniqueEmail = true;

            // Default Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        }).AddRoles<IdentityRole>()
          .AddEntityFrameworkStores<IdentityContext>();

        builder.Services.AddTransient<IEmailSender, EmailSender>();
        //builder.Services.AddTransient<IEmailSender, FakeEmailSender>();

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        builder.Services.AddTransient<IAtendimentoService, AtendimentoService>();
        builder.Services.AddTransient<IAvaliacaoService, AvaliacaoService>();
        builder.Services.AddTransient<IItemService, ItemService>();
        builder.Services.AddTransient<IClienteService, ClienteService>();
        builder.Services.AddTransient<IPedidoService, PedidoService>();
        builder.Services.AddTransient<IEnderecoService, EnderecoService>();
        builder.Services.AddTransient<IRestauranteService, RestauranteService>();
        builder.Services.AddTransient<IPagamentoService, PagamentoService>();
        builder.Services.AddTransient<ICategoriaService, CategoriaService>();
        builder.Services.AddTransient<ICartaoService, CartaoService>();
        builder.Services.AddTransient<ICarrinhoService, CarrinhoService>();

        builder.Services.ConfigureApplicationCookie(options =>
                {
                    //options.AccessDeniedPath = "/Identity/Autenticar";
                    options.Cookie.Name = "DeliFitCookieName";
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    //options.LoginPath = "/Identity/Autenticar";
                    // ReturnUrlParameter requires 
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                    options.SlidingExpiration = true;
        });


        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

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

        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");


        using (var scope = app.Services.CreateScope())
        {
            
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UsuarioIdentity>>();

            string[] roleNames = { "Admin", "GerenteRestaurante", "Cliente" };

            foreach (var roleName in roleNames)
            {
                // Verifica se a role já existe, se não, cria.
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }


        app.Run();
    }

    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Retorna logo, sem tentar acessar nenhum servidor SMTP de fato.
            return Task.CompletedTask;
        }
    }

}