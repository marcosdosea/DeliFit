using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace DeliFitWeb.Controllers.Tests
{
    [TestClass]
    public class CartaoControllerTests
    {
        private CartaoController controller = null!;
        private Mock<ICartaoService> mockService = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockService = new Mock<ICartaoService>();
            var mockClienteService = new Mock<IClienteService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new CartaoProfile())).CreateMapper();

            mockService.Setup(s => s.GetByCliente(It.IsAny<uint>()))
                .Returns(GetTestCartaos());

            mockService.Setup(s => s.Get(It.IsAny<uint>()))
                .Returns(GetTargetCartao());

            mockService.Setup(s => s.Create(It.IsAny<Cartao>()));
            mockService.Setup(s => s.Delete(It.IsAny<uint>()));

            controller = new CartaoController(mockService.Object, mapper, mockClienteService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "teste@email.com")
            }, "mock"));

            var httpContext = new DefaultHttpContext { User = user };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            var result = controller.Index(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var lista = viewResult.Model as List<CartaoViewModel>;

            Assert.IsNotNull(lista);
            Assert.AreEqual(3, lista.Count);
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var model = ((ViewResult)result).Model as CartaoViewModel;

            Assert.IsNotNull(model);
            Assert.AreEqual("Kauan Brilhante", model.Nome);
        }

        [TestMethod]
        public void CreateTest_Get_Valido()
        {
            var result = controller.Create(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CreateTest_Post_Valid()
        {
            var result = controller.Create(GetNewCartao());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public void CreateTest_Post_Invalid()
        {
            controller.ModelState.AddModelError("Numero", "Campo requerido");

            var result = controller.Create(GetNewCartao());

            // Formulário inválido reexibe a própria tela de cadastro com os erros
            // ao lado dos campos, em vez de redirecionar para a lista sem explicação.
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void DeleteTest_Get_Valid()
        {
            var result = controller.Delete(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void DeleteTest_Post_Valid()
        {
            var model = GetTargetCartaoViewModel();
            var result = controller.Delete(model.Id, model);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        private CartaoViewModel GetNewCartao()
        {
            return new CartaoViewModel
            {
                Id = 4,
                IdCliente = 1,
                Nome = "Novo Cartao",
                Cpf = "12312312312",
                Numero = "1234123412341234",
                Validade = new DateTime(2030, 5, 1),
                Cvv = "321"
            };
        }

        private CartaoViewModel GetTargetCartaoViewModel()
        {
            return new CartaoViewModel
            {
                Id = 1,
                IdCliente = 1,
                Nome = "Kauan Brilhante",
                Cpf = "11111111111",
                Numero = "1111222233334444",
                Validade = new DateTime(2027, 12, 1),
                Cvv = "123"
            };
        }

        private Cartao GetTargetCartao()
        {
            return new Cartao
            {
                Id = 1,
                IdCliente = 1,
                Nome = "Kauan Brilhante",
                Cpf = "11111111111",
                Numero = "1111222233334444",
                Validade = new DateTime(2027, 12, 1),
                Cvv = "123"
            };
        }

        private IEnumerable<Cartao> GetTestCartaos()
        {
            return new List<Cartao>
            {
                new Cartao
                {
                    Id = 1,
                    IdCliente = 1,
                    Nome = "Kauan Brilhante",
                    Cpf = "11111111111",
                    Numero = "1111222233334444",
                    Validade = new DateTime(2027, 12, 1),
                    Cvv = "123"
                },
                new Cartao
                {
                    Id = 2,
                    IdCliente = 1,
                    Nome = "Kauan Brilhante",
                    Cpf = "22222222222",
                    Numero = "5555666677778888",
                    Validade = new DateTime(2028, 6, 1),
                    Cvv = "456"
                },
                new Cartao
                {
                    Id = 3,
                    IdCliente = 1,
                    Nome = "Outro Nome",
                    Cpf = "33333333333",
                    Numero = "9999000011112222",
                    Validade = new DateTime(2029, 1, 1),
                    Cvv = "789"
                }
            };
        }
    }
}