using AutoMapper;
using Core;
using Core.DTO;
using Core.Identity.Data;
using Core.Service;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace DeliFitWeb.Controllers.Tests
{
    [TestClass]
    public class RestauranteControllersTests
    {
        private RestauranteController? controller;
        private Mock<IRestauranteService>? mockService;

        [TestInitialize]
        public void Initialize()
        {
            mockService = new Mock<IRestauranteService>();

            var mockUserManager = new Mock<UserManager<UsuarioIdentity>>(
                Mock.Of<IUserStore<UsuarioIdentity>>(), null, null, null, null, null, null, null, null);

            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            var mockEmailSender = new Mock<IEmailSender>();

            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((UsuarioIdentity?)null);
            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UsuarioIdentity>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<UsuarioIdentity>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<UsuarioIdentity>()))
                .ReturnsAsync(IdentityResult.Success);

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile(new RestauranteProfile())).CreateMapper();

            mockService.Setup(service => service.GetRestaurantesAtivos())
                .Returns(GetTestRestaurantes().Where(r => r.Validado).ToList());

            mockService.Setup(service => service.GetRestaurantesPendentes())
                .Returns(GetTestRestaurantes().Where(r => !r.Validado).ToList());

            mockService.Setup(service => service.Get(1)).Returns(GetTargetRestaurante());

            mockService.Setup(service => service.Edit(It.IsAny<Restaurante>())).Verifiable();
            mockService.Setup(service => service.Create(It.IsAny<Restaurante>())).Verifiable();
            mockService.Setup(service => service.Delete(It.IsAny<uint>())).Verifiable();

            controller = new RestauranteController(
                mockService.Object,
                mapper,
                mockUserManager.Object,
                mockRoleManager.Object,
                mockEmailSender.Object);

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

        private static RestauranteViewModel GetNewRestaurante()
        {
            return new RestauranteViewModel
            {
                Id = 4,
                NomeRestaurante = "Teste Restaurante 4",
                Validado = false,
                Cidade = "Cidade 4",
                Estado = "Estado 4",
            };
        }

        private static Restaurante GetTargetRestaurante()
        {
            return new Restaurante
            {
                Id = 1,
                NomeRestaurante = "Teste Restaurante 1",
                Validado = true,
                Cidade = "Cidade 1",
                Estado = "Estado 1",
                NomeProprietario = "Proprietario 1",
            };
        }

        private static RestauranteViewModel GetTargetRestauranteViewModel()
        {
            return new RestauranteViewModel
            {
                Id = 1,
                NomeRestaurante = "Teste Restaurante 1",
                Validado = true,
                Cidade = "Cidade 1",
                Estado = "Estado 1",
                NomeProprietario = "Proprietario 1",
            };
        }

        private static List<RestauranteDTO> GetTestRestaurantes()
        {
            return new List<RestauranteDTO>
            {
                new() {
                    Id = 1,
                    NomeRestaurante = "Teste Restaurante 1",
                    Validado = true,
                    Cidade = "Cidade 1",
                    Estado = "Estado 1"
                },
                new()
                {
                    Id = 2,
                    NomeRestaurante = "Teste Restaurante 2",
                    Validado = false,
                    Cidade = "Cidade 2",
                    Estado = "Estado 2"
                },
                new RestauranteDTO
                {
                    Id = 3,
                    NomeRestaurante = "Teste Restaurante 3",
                    Validado = true,
                    Cidade = "Cidade 3",
                    Estado = "Estado 3"
                },
            };
        }

        [TestMethod]
        [TestCategory("Unit")]
        [Description("Testando o Index")]
        public void IndexTest()
        {
            var result = controller!.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(List<RestauranteViewModel>));
            List<RestauranteViewModel> viewModel = (List<RestauranteViewModel>)viewResult.Model;

            Assert.AreEqual(2, viewModel.Count);
        }

        [TestMethod]
        public void DetailsTest()
        {
            var result = controller!.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(RestauranteViewModel));
            RestauranteViewModel viewModel = (RestauranteViewModel)viewResult.Model;

            Assert.AreEqual((uint)1, viewModel.Id);
            Assert.AreEqual("Teste Restaurante 1", viewModel.NomeRestaurante);
            Assert.IsTrue(viewModel.Validado);
            Assert.AreEqual("Cidade 1", viewModel.Cidade);
            Assert.AreEqual("Estado 1", viewModel.Estado);
            Assert.AreEqual("Proprietario 1", viewModel.NomeProprietario);
        }

        [TestMethod]
        public void CreateTest_Get_Valido()
        {
            var result = controller!.Create();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CreateTest_Valid()
        {
            var result = controller!.Create(GetNewRestaurante());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;

            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod]
        public void CreateTest_Post_InValid()
        {
            controller!.ModelState.AddModelError("Nome", "Campo requerido");

            var result = controller!.Create(GetNewRestaurante());

            Assert.AreEqual(1, controller.ModelState.ErrorCount);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod]
        public void EditTest_Get_Valid()
        {
            var result = controller!.Edit(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(RestauranteViewModel));
            RestauranteViewModel restauranteModel = (RestauranteViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("Teste Restaurante 1", restauranteModel.NomeRestaurante);
            Assert.AreEqual("Cidade 1", restauranteModel.Cidade);
            Assert.AreEqual("Estado 1", restauranteModel.Estado);
            Assert.AreEqual("Proprietario 1", restauranteModel.NomeProprietario);
        }

        [TestMethod]
        public void EditTest_Post_Valid()
        {
            var result = controller!.Edit(GetTargetRestauranteViewModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod]
        public void DeleteTest_Get_Valid()
        {
            var result = controller!.Delete(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(RestauranteViewModel));
            RestauranteViewModel restauranteModel = (RestauranteViewModel)viewResult.ViewData.Model;
            Assert.AreEqual("Teste Restaurante 1", restauranteModel.NomeRestaurante);
            Assert.AreEqual("Cidade 1", restauranteModel.Cidade);
            Assert.AreEqual("Estado 1", restauranteModel.Estado);
            Assert.AreEqual("Proprietario 1", restauranteModel.NomeProprietario);
            Assert.IsTrue(restauranteModel.Validado);
        }

        [TestMethod]
        public void DeleteTest_Post_Valid()
        {
            var result = Unwrap(controller!.Delete((uint)1, GetTargetRestauranteViewModel()));

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private static object? Unwrap(object? maybeTask)
        {
            if (maybeTask is null) return null;
            if (maybeTask is Task task)
            {
                task.GetAwaiter().GetResult();

                var taskType = task.GetType();
                if (taskType.IsGenericType)
                {
                    var prop = taskType.GetProperty("Result");
                    return prop?.GetValue(task);
                }

                return null;
            }

            return maybeTask;
        }
    }
}