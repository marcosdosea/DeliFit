using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using DeliFitWeb.Areas.Identity.Data;
using System.Security.Cryptography;
using System.Linq;

namespace DeliFitWeb.Controllers
{
    public class RestauranteController : Controller
    {
        private readonly IRestauranteService _restauranteService;
        private readonly IMapper _mapper;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public RestauranteController(IRestauranteService restauranteService,
                                     IMapper mapper,
                                     UserManager<UsuarioIdentity> userManager,
                                     RoleManager<IdentityRole> roleManager,
                                     IEmailSender emailSender)
        {
            _restauranteService = restauranteService;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        private static string GenerateSecurePassword(int length = 12)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string digits = "0123456789";
            if (length < 6) length = 6;

            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            var chars = bytes.Select(b => valid[b % valid.Length]).ToArray();

            // ensure at least one digit
            if (!chars.Any(c => digits.Contains(c)))
            {
                var pos = Math.Abs(BitConverter.ToInt32(bytes, 0)) % length;
                chars[pos] = digits[bytes[0] % digits.Length];
            }

            return new string(chars);
        }

        // GET: RestauranteController
        public ActionResult Index()
        {
            var listaRestaurantes = _restauranteService.GetRestaurantesAtivos();
            var listaRestaurantesModel = _mapper.Map<List<RestauranteViewModel>>(listaRestaurantes);

            return View(listaRestaurantesModel);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult ListarSolicitacoes()
        {
            var listaRestaurantes = _restauranteService.GetRestaurantesPendentes();
            var listaRestaurantesModel = _mapper.Map<List<RestauranteViewModel>>(listaRestaurantes);

            return View(listaRestaurantesModel);
        }

        // GET: RestauranteController/Details/5
        public ActionResult Details(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            ViewBag.CanChangeStatus = false;

            return View(restauranteModel);
        }

        [Authorize(Roles = "Admin")]
        // GET: RestauranteController/DetailsSolicitacao/5
        public ActionResult DetailsSolicitacao(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            ViewBag.CanChangeStatus = true;

            return View(restauranteModel);
        }

        // GET: RestauranteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestauranteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RestauranteViewModel restauranteModel)
        {

            if (ModelState.IsValid)
            {
                var restaurante = _mapper.Map<Restaurante>(restauranteModel);
                _restauranteService.Create(restaurante);
                return RedirectToAction(nameof(Index));
            }

            return View(restauranteModel);
        }

        // GET: RestauranteController/Edit/5
        public ActionResult Edit(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            return View(restauranteModel);
        }

        // POST: RestauranteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RestauranteViewModel restauranteModel)
        {
            if (ModelState.IsValid)
            {
                var restaurante = _mapper.Map<Restaurante>(restauranteModel);
                _restauranteService.Edit(restaurante);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: RestauranteController/Delete/5
        public ActionResult Delete(uint id)
        {
            Restaurante? restaurante = _restauranteService.Get(id);
            RestauranteViewModel restauranteModel = _mapper.Map<RestauranteViewModel>(restaurante);

            return View(restauranteModel);
        }

        // POST: RestauranteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, RestauranteViewModel restauranteModel)
        {
            _restauranteService.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        // GET: RestauranteController/SolicitarAdesao
        public ActionResult SolicitarAdesao()
        {
            return View();
        }

        // POST: RestauranteController/SolicitarAdesao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SolicitarAdesao(RestauranteViewModel restauranteModel)
        {

            if (ModelState.IsValid)
            {
                var restaurante = _mapper.Map<Restaurante>(restauranteModel);
                _restauranteService.Create(restaurante);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: RestauranteController/AprovarSolicitacao/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AprovarSolicitacao(uint id)
        {
            var restaurante = _restauranteService.Get(id);
            if (restaurante != null)
            {
                // Cria usuário Identity para o restaurante com senha gerada
                var email = restaurante.Email;
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var user = new UsuarioIdentity { UserName = email, Email = email };
                    var senha = GenerateSecurePassword(12);
                    var createResult = await _userManager.CreateAsync(user, senha);
                    if (createResult.Succeeded)
                    {
                        // Atribui role e ativa restaurante
                        await _userManager.AddToRoleAsync(user, "GerenteRestaurante");

                        restaurante.Validado = true; // Ativa o restaurante
                        _restauranteService.Edit(restaurante);

                        // Envia e-mail com credenciais (opção B: menos seguro)
                        var assunto = "Solicitação aprovada - DeliFit";
                        var mensagem = $"Sua solicitação foi aprovada.\nUsuário: {email}\nSenha: {senha}\nAcesse: {Request.Scheme}://{Request.Host}/Identity/Account/Login";
                        await _emailSender.SendEmailAsync(email, assunto, mensagem);
                    }
                    else
                    {
                        // Em caso de erro ao criar usuário, você pode logar os erros
                        // por enquanto não altera o estado do restaurante
                    }
                }
                else
                {
                    // Email não informado, não é possível criar usuário
                }
            }
            return RedirectToAction(nameof(ListarSolicitacoes));
        }

        // POST: RestauranteController/NegarSolicitacao/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NegarSolicitacao(uint id)
        {
            _restauranteService.Delete(id);

            return RedirectToAction(nameof(ListarSolicitacoes));
        }
    }
}
