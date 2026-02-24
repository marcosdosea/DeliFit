// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Core.Service;
using Core.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliFitWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ClienteLoginModel : PageModel
    {
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClienteLoginModel> _logger;

        public ClienteLoginModel(
            SignInManager<UsuarioIdentity> signInManager,
            UserManager<UsuarioIdentity> userManager,
            IClienteService clienteService,
            ILogger<ClienteLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _clienteService = clienteService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Informe o e-mail ou telefone.")]
            [Display(Name = "E-mail ou Telefone")]
            public string EmailOuTelefone { get; set; }

            [Required(ErrorMessage = "Informe a senha.")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            [Display(Name = "Lembrar-me")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Restaurante/Index");

            if (!ModelState.IsValid)
                return Page();

            // Resolve o e-mail do Identity a partir do e-mail ou telefone informado
            string emailIdentity = ResolverEmailIdentity(Input.EmailOuTelefone);

            if (emailIdentity == null)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou telefone não encontrado.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(emailIdentity, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(emailIdentity);
                var roles = await _userManager.GetRolesAsync(user);

                // Garante que somente clientes acessem este fluxo
                if (!roles.Contains("Cliente"))
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "Acesso negado. Utilize o login correspondente ao seu perfil.");
                    return Page();
                }

                _logger.LogInformation("Cliente autenticado com sucesso.");
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Conta de cliente bloqueada.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Tentativa de login inválida.");
            return Page();
        }

        /// <summary>
        /// Dado um valor que pode ser e-mail ou telefone, retorna o e-mail
        /// registrado no Identity para uso no PasswordSignInAsync.
        /// </summary>
        private string ResolverEmailIdentity(string emailOuTelefone)
        {
            if (emailOuTelefone.Contains('@'))
            {
                var cliente = _clienteService.GetByEmail(emailOuTelefone);
                return cliente?.Email;
            }

            var clientePorTelefone = _clienteService.GetByTelefone(emailOuTelefone);
            return clientePorTelefone?.Email;
        }
    }
}