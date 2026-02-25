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
    public class EnderecoControllerTests
    {
        private EnderecoController controller = null!;
        private Mock<IEnderecoService> mockService = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockService = new Mock<IEnderecoService>();
            var mockClienteService = new Mock<IClienteService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new EnderecoProfile())).CreateMapper();

            mockService.Setup(s => s.GetAll())
                .Returns(GetTestEnderecoes());

            mockService.Setup(s => s.Get(It.IsAny<uint>()))
                .Returns(GetTargetEndereco());

            mockService.Setup(s => s.Create(It.IsAny<Endereco>()));
            mockService.Setup(s => s.Edit(It.IsAny<Endereco>()));
            mockService.Setup(s => s.Delete(It.IsAny<uint>()));

            controller = new EnderecoController(mockService.Object, mapper, mockClienteService.Object);

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
            var lista = viewResult.Model as List<EnderecoViewModel>;

            Assert.IsNotNull(lista);
            Assert.AreEqual(3, lista.Count);
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var model = ((ViewResult)result).Model as EnderecoViewModel;

            Assert.IsNotNull(model);
            Assert.AreEqual("Rua 1", model.Rua);
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
            var result = controller.Create(GetNewEndereco());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public void CreateTest_Post_Invalid()
        {
            controller.ModelState.AddModelError("Rua", "Campo requerido");

            var result = controller.Create(GetNewEndereco());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void EditTest_Get_Valid()
        {
            var result = controller.Edit(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void EditTest_Post_Valid()
        {
            var model = GetTargetEnderecoViewModel();
            var result = controller.Edit(model.Id, model);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
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
            var model = GetTargetEnderecoViewModel();
            var result = controller.Delete(model.Id, model);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        private EnderecoViewModel GetNewEndereco()
        {
            return new EnderecoViewModel
            {
                Id = 4,
                IdCliente = 1,
                Rua = "Av Brasil",
                Numero = "123",
                Bairro = "Centro",
                Cep = "12345-678",
                Cidade = "São Paulo",
                Estado = "SP",
                Label = "Casa"
            };
        }

        private EnderecoViewModel GetTargetEnderecoViewModel()
        {
            return new EnderecoViewModel
            {
                Id = 1,
                IdCliente = 1,
                Rua = "Rua 1",
                Numero = "10",
                Bairro = "Bela Vista",
                Cep = "01000-000",
                Cidade = "São Paulo",
                Estado = "SP",
                Label = "Trabalho"
            };
        }

        private Endereco GetTargetEndereco()
        {
            return new Endereco
            {
                Id = 1,
                IdCliente = 1,
                Rua = "Rua 1",
                Numero = "10",
                Bairro = "Bela Vista",
                Cep = "01000-000",
                Cidade = "São Paulo",
                Estado = "SP",
                Label = "Trabalho"
            };
        }

        private IEnumerable<Endereco> GetTestEnderecoes()
        {
            return new List<Endereco>
            {
                new Endereco
                {
                    Id = 1,
                    IdCliente = 1,
                    Rua = "Graciliano Ramos",
                    Numero = "1",
                    Bairro = "Centro",
                    Cep = "11111-111",
                    Cidade = "São Paulo",
                    Estado = "SP"
                },
                new Endereco
                {
                    Id = 2,
                    IdCliente = 1,
                    Rua = "Machado de Assis",
                    Numero = "2",
                    Bairro = "Centro",
                    Cep = "22222-222",
                    Cidade = "São Paulo",
                    Estado = "SP"
                },
                new Endereco
                {
                    Id = 3,
                    IdCliente = 1,
                    Rua = "Marcos Dósea",
                    Numero = "3",
                    Bairro = "Centro",
                    Cep = "33333-333",
                    Cidade = "São Paulo",
                    Estado = "SP"
                }
            };
        }
    }
}