using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DeliFitWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // C#
        public IActionResult Index()
        {
            var model = new RestauranteViewModel
            {
                Id = 1,
                NomeRestaurante = "Restaurante 1",
                Cidade = "Cidade 1",
                Bairro = "Bairro",
                Cep = "12345-789",
                Cnpj = "12345678910121",
                CpfProprietario = "12345678901",
                Email = "teste@gmail.com",
                Estado = "Estado",
                Numero = "123",
                Rua = "Rua",
                TelefoneProprietario = "79999419916",
                TelefoneRestaurante = "79999419916",
                NomeProprietario = "NomeTeste"
            }; // preencher conforme necessário
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}