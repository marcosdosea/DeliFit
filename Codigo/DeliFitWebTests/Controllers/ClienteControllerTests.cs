using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DeliFitWeb.Controllers.Tests
{
    [TestClass()]
    public class ClienteControllerTests
    {
        private static ClienteController? controller;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var mockService = new Mock<IClienteService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new ClienteProfile())).CreateMapper();

            mockService.Setup(service => service.GetAll())
                .Returns(GetTestCliente());
            mockService.Setup(service => service.Get(1))
                .Returns(GetTargetCliente());
            mockService.Setup(service => service.Edit(It.IsAny<Cliente>()))
                .Verifiable();
            mockService.Setup(service => service.Create(It.IsAny<Cliente>()))
                .Verifiable();
            controller = new ClienteController(mockService.Object, mapper);
        }

        [TestMethod()]
        [TestCategory("Unit")]
        [Description("Testando o index")]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller?.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<ClienteViewModel>));

            List<ClienteViewModel>? lista = (List<ClienteViewModel>)viewResult.ViewData.Model;
            Assert.HasCount(3, lista);
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller?.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ClienteViewModel));
            ClienteViewModel clienteModel = (ClienteViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("Machado de Assis", clienteModel.Nome);
            Assert.AreEqual("79988888888", clienteModel.Telefone);
        }

        [TestMethod()]
        public void CreateTest_Get_Valido()
        {
            // Act
            var result = controller?.Create();
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void CreateTest_Valid()
        {
            // Act
            var result = controller?.Create(GetNewCliente());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void CreateTest_Post_Invalid()
        {
            // Arrange
            controller?.ModelState.AddModelError("Nome", "Nome é obrigatório.");

            // Act
            var result = controller?.Create(GetNewCliente());

            // Assert
            Assert.AreEqual(1, controller?.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void EditTest_Get_Valid()
        {
            // Act
            var result = controller?.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ClienteViewModel));
            ClienteViewModel clienteModel = (ClienteViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("Machado de Assis", clienteModel.Nome);
            Assert.AreEqual("79988888888", clienteModel.Telefone);
        }

        [TestMethod()]
        public void EditTest_Post_Valid()
        {
            // Act
            var result = controller?.Edit(GetTargetClienteModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void DeleteTest_Post_Valid()
        {
            // Act
            var result = controller?.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ClienteViewModel));
            ClienteViewModel clienteModel = (ClienteViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("Machado de Assis", clienteModel.Nome);
            Assert.AreEqual("79988888888", clienteModel.Telefone);
        }

        [TestMethod()]
        public void DeleteTest_Get_Valid()
        {
            // Act
            var result = controller?.Delete(GetTargetClienteModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private ClienteViewModel GetNewCliente()
        {
            return new ClienteViewModel
            {
                Id = 4,
                Nome = "Ian Sommerville",
                Telefone = "79966666666"
            };

        }
        private static Cliente GetTargetCliente()
        {
            return new Cliente
            {
                Id = 1,
                Nome = "Machado de Assis",
                Telefone = "79988888888"
            };
        }

        private ClienteViewModel GetTargetClienteModel()
        {
            return new ClienteViewModel
            {
                Id = 2,
                Nome = "Machado de Assis",
                Telefone = "79988888888"
            };
        }

        private IEnumerable<ClienteDTO> GetTestCliente()
        {
            return new List<ClienteDTO>
            {
                new ClienteDTO
                {
                    Id = 1,
                    Nome = "Graciliano Ramos",
                    Telefone = "79999999999"
                },
                new ClienteDTO
                {
                    Id = 2,
                    Nome = "Machado de Assis",
                    Telefone = "79988888888"
                },
                new ClienteDTO
                {
                    Id = 3,
                    Nome = "Marcos Dósea",
                    Telefone = "79977777777"
                },
            };
        }
    }
}