using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using DeliFitWeb.Areas.Identity.Data;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Identity;
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
            var mockBrasilApi = new Mock<IBrasilApiService>();
            var mockUserManager = new Mock<UserManager<UsuarioIdentity>>(
                new Mock<IUserStore<UsuarioIdentity>>().Object,
                null, null, null, null, null, null, null, null);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new ClienteProfile())).CreateMapper();

            mockBrasilApi
                .Setup(service => service.IsDddValidAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Configura comportamento do mock do IClienteService para os testes funcionarem
            mockService
                .Setup(service => service.GetAll())
                .Returns(GetTestCliente());

            mockService
                .Setup(service => service.Get(It.Is<uint>(id => id == 1)))
                .Returns(GetTargetCliente());

            mockService
                .Setup(service => service.Get(It.Is<uint>(id => id != 1)))
                .Returns((Cliente?)null);

            mockService
                .Setup(service => service.GetByEmail(It.IsAny<string>()))
                .Returns((string email) => {
                    // tenta encontrar um ClienteDTO com email correspondente (não existe em dados de teste)
                    return null;
                });

            controller = new ClienteController(
                mockService.Object,
                mapper,
                mockBrasilApi.Object,
                mockUserManager.Object
            );
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
            var novoClienteViewModel = GetNewCliente();
            var result = Unwrap(controller?.CreateAsync(novoClienteViewModel));

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
            var result = Unwrap(controller?.CreateAsync(GetNewCliente()));
            // Assert
            Assert.AreEqual(1, controller?.ModelState.ErrorCount);
            // When model state is invalid the controller should re-display the Create view
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ClienteViewModel));
            ClienteViewModel clienteModel = (ClienteViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(4u, clienteModel.Id);
            Assert.AreEqual("Ian Sommerville", clienteModel.Nome);
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


        private static object? Unwrap(object? maybeTask)
        {
            if (maybeTask is null) return null;
            if (maybeTask is Task task)
            {
                // Aguarda término
                task.GetAwaiter().GetResult();

                var taskType = task.GetType();
                if (taskType.IsGenericType)
                {
                    // Obtém propriedade Result via reflexão para Task<T>
                    var prop = taskType.GetProperty("Result");
                    return prop?.GetValue(task);
                }

                // Task sem resultado
                return null;
            }

            return maybeTask;
        }
    }
}