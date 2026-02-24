// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DeliFitWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DeliFitWeb.Helpers;
using Core.Service;

namespace DeliFitWeb.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IClienteService _clienteService;
        private readonly IRestauranteService _restauranteService;

        public LoginModel(SignInManager<UsuarioIdentity> signInManager, 
                         ILogger<LoginModel> logger, 
                         UserManager<UsuarioIdentity> userManager,
                         IClienteService clienteService,
                         IRestauranteService restauranteService)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _clienteService = clienteService;
            _restauranteService = restauranteService;
        }

        // Optional incoming role from query string (propagated to Register link)
        public string Role { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O telefone é obrigatório.")]
            [Display(Name = "Telefone")]
            public string Telefone { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null, string role = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
            Role = role; // store role to propagate to Register link
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Normaliza telefone para dígitos
                var digits = new string((Input.Telefone ?? string.Empty).Where(char.IsDigit).ToArray());
                if (digits.Length != 11)
                {
                    ModelState.AddModelError(string.Empty, "O telefone deve conter 11 dígitos.");
                    return Page();
                }

                // Procura usuário pelo PhoneNumber
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == digits);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Credenciais inválidas.");
                    return Page();
                }

                // Usa o UserName do usuário encontrado para efetuar o sign-in
                var usernameForSignIn = user.UserName ?? user.Email;
                var result = await _signInManager.PasswordSignInAsync(usernameForSignIn, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Pega as roles (perfis) desse usuário
                    var roles = await _userManager.GetRolesAsync(user);

                    // Armazena email na sessão
                    HttpContext.Session.SetUserEmail(Input.Email);

                    // Redireciona de acordo com a Role e armazena dados na sessão
                    if (roles.Contains("Admin"))
                    {
                        HttpContext.Session.SetUserRole("Admin");
                        return LocalRedirect("~/Restaurante/HomeAdmin"); 
                    }
                    if (roles.Contains("GerenteRestaurante"))
                    {
                        HttpContext.Session.SetUserRole("GerenteRestaurante");

                        // Busca e armazena ID do restaurante
                        var restaurante = _restauranteService.GetByEmail(Input.Email);
                        if (restaurante != null)
                        {
                            HttpContext.Session.SetRestauranteId(restaurante.Id);
                        }

                        return LocalRedirect("~/Restaurante/Home"); 
                    }
                    if (roles.Contains("Cliente"))
                    {
                        HttpContext.Session.SetUserRole("Cliente");

                        // Busca e armazena ID do cliente
                        var cliente = _clienteService.GetByEmail(Input.Email);
                        if (cliente != null)
                        {
                            HttpContext.Session.SetClienteId(cliente.Id);
                        }

                        return LocalRedirect("~/Cliente/HomeCliente");
                    }

                    return LocalRedirect(returnUrl ?? "~/");

                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            return Page();
        }
    }
}