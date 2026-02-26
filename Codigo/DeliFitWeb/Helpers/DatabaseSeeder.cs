using Core;
using Core.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DeliFitWeb.Helpers;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        UserManager<UsuarioIdentity> userManager,
        RoleManager<IdentityRole> roleManager,
        DeliFitContext context)
    {
        await SeedRolesAsync(roleManager);
        await SeedAdminAsync(userManager);
        await SeedClienteAsync(userManager, context);
        await SeedRestauranteAsync(userManager, context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "GerenteRestaurante", "Cliente" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedAdminAsync(UserManager<UsuarioIdentity> userManager)
    {
        const string email = "admin@delifit.com";
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var user = new UsuarioIdentity { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, "Admin@123");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, "Admin");
    }

    private static async Task SeedClienteAsync(
        UserManager<UsuarioIdentity> userManager,
        DeliFitContext context)
    {
        const string email = "cliente@delifit.com";
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var user = new UsuarioIdentity
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            PhoneNumber = "62987654321"
        };

        var result = await userManager.CreateAsync(user, "Senha@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Cliente");

            if (!await context.Clientes.AnyAsync(c => c.Cpf == "52998224725"))
            {
                context.Clientes.Add(new Cliente
                {
                    Nome = "João Silva",
                    Email = email,
                    Cpf = "52998224725",
                    Telefone = "62987654321",
                    DataNascimento = new DateTime(1990, 5, 15)
                });
                await context.SaveChangesAsync();
            }
        }
    }

    private static async Task SeedRestauranteAsync(
        UserManager<UsuarioIdentity> userManager,
        DeliFitContext context)
    {
        const string email = "restaurante@delifit.com";
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var user = new UsuarioIdentity
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            PhoneNumber = "62912345678"
        };

        var result = await userManager.CreateAsync(user, "Senha@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "GerenteRestaurante");

            if (!await context.Restaurantes.AnyAsync(r => r.Cnpj == "11222333000181"))
            {
                context.Restaurantes.Add(new Restaurante
                {
                    NomeRestaurante = "Sabor Natural",
                    NomeProprietario = "Maria Santos",
                    CpfProprietario = "11144477735",
                    Cnpj = "11222333000181",
                    Descricao = "Restaurante saudável com pratos naturais e nutritivos.",
                    TelefoneProprietario = "62912345678",
                    TelefoneRestaurante = "6232109876",
                    Email = email,
                    Validado = true,
                    Rua = "Rua das Flores",
                    Numero = "123",
                    Bairro = "Jardim América",
                    Cep = "74000000",
                    Cidade = "Goiânia",
                    Estado = "GO"
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
