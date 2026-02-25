using DeliFitAPI.Filter;
using Core;
using Core.Identity.Data;
using Core.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service;

namespace DeliFitAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container with Exception handling.
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
                options.Filters.Add(new CartaoValidationFilter());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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


            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            // Configuração do AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Registro dos serviços
            builder.Services.AddScoped<IClienteService, ClienteService>();
            builder.Services.AddScoped<IRestauranteService, RestauranteService>();
            builder.Services.AddScoped<IEnderecoService, EnderecoService>();
            builder.Services.AddScoped<ICartaoService, CartaoService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapIdentityApi<UsuarioIdentity>();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}