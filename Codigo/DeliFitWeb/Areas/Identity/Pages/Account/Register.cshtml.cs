// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using DeliFitWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Core;
using Core.Service;
using System.Linq;

namespace DeliFitWeb.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IUserStore<UsuarioIdentity> _userStore;
        private readonly IUserEmailStore<UsuarioIdentity> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IClienteService _clienteService;

        public RegisterModel(
            UserManager<UsuarioIdentity> userManager,
            IUserStore<UsuarioIdentity> userStore,
            SignInManager<UsuarioIdentity> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IClienteService clienteService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _clienteService = clienteService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Tipo de Perfil")]
            public string RoleDesejada { get; set; }

            // Campos extras para cadastro de Cliente
            [Required(ErrorMessage = "Campo requerido.")]
            [Display(Name = "Nome completo")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 caracteres.")]
            public string Nome { get; set; }

            [Required(ErrorMessage = "Campo requerido")]
            [Display(Name = "CPF")]
            [StringLength(11, MinimumLength = 11, ErrorMessage = "O cpf deve conter 11 caracteres.")]
            [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos.")]
            public string Cpf { get; set; }

            [Required(ErrorMessage = "Campo requerido")]
            [Display(Name = "Telefone")]
            [RegularExpression(@"^(?:\(?\d{2}\)?\s?\d{4,5}-?\d{4}|\d{11})$", ErrorMessage = "O telefone deve conter exatamente 11 dígitos.")]
            public string Telefone { get; set; }

            [Display(Name = "Data de Nascimento")]
            [DataType(DataType.Date)]
            public DateTime? DataNascimento { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null, string role = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Input ??= new InputModel();
            Input.RoleDesejada ??= string.IsNullOrEmpty(role) ? "Cliente" : role;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Normalização de campos que podem vir formatados (ex.: máscara)
            if (Input != null)
            {
                if (!string.IsNullOrWhiteSpace(Input.Telefone))
                    Input.Telefone = new string(Input.Telefone.Where(char.IsDigit).ToArray());
                if (!string.IsNullOrWhiteSpace(Input.Cpf))
                    Input.Cpf = new string(Input.Cpf.Where(char.IsDigit).ToArray());
                if (!string.IsNullOrWhiteSpace(Input.Nome))
                    Input.Nome = Input.Nome.Trim();
            }

            // Validações extras para role Cliente
            if (Input.RoleDesejada == "Cliente")
            {
                if (string.IsNullOrWhiteSpace(Input.Nome))
                    ModelState.AddModelError("Input.Nome", "O nome é obrigatório.");
                if (string.IsNullOrWhiteSpace(Input.Cpf) || Input.Cpf.Length != 11 || !Input.Cpf.All(char.IsDigit))
                    ModelState.AddModelError("Input.Cpf", "O CPF é obrigatório e deve conter 11 dígitos.");
                if (string.IsNullOrWhiteSpace(Input.Telefone) || Input.Telefone.Length != 11 || !Input.Telefone.All(char.IsDigit))
                    ModelState.AddModelError("Input.Telefone", "O telefone é obrigatório e deve conter 11 dígitos.");
                if (Input.DataNascimento == null)
                    ModelState.AddModelError("Input.DataNascimento", "A data de nascimento é obrigatória.");
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Salva telefone em AspNetUsers.PhoneNumber se disponível
                    if (!string.IsNullOrWhiteSpace(Input.Telefone))
                    {
                        var phoneResult = await _userManager.SetPhoneNumberAsync(user, Input.Telefone);
                        if (!phoneResult.Succeeded)
                        {
                            // registra erro e desfaz criação do cliente no banco se necessário
                            foreach (var err in phoneResult.Errors)
                                ModelState.AddModelError(string.Empty, err.Description);

                            // Trata falha como erro fatal: não prossegue com role, cliente, e login.
                            return Page();
                        }
                    }

                    await _userManager.AddToRoleAsync(user, Input.RoleDesejada);

                    // Se for cliente, cria o registro na tabela Cliente
                    if (Input.RoleDesejada == "Cliente")
                    {
                        var cliente = new Cliente
                        {
                            Nome = Input.Nome,
                            Email = Input.Email,
                            Cpf = Input.Cpf,
                            Telefone = Input.Telefone,
                            DataNascimento = Input.DataNascimento!.Value
                        };
                        _clienteService.Create(cliente);
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private UsuarioIdentity CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UsuarioIdentity>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UsuarioIdentity)}'. " +
                    $"Ensure that '{nameof(UsuarioIdentity)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<UsuarioIdentity> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<UsuarioIdentity>)_userStore;
        }
    }
}