using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Controllers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DeliFitWeb.Mappers;
using System.Security.Claims;
using System.Text.Json;

namespace DeliFitWeb.Controllers.Tests
{
    [TestClass()]
    public class PedidoControllerTests
    {
        private PedidoController? controller;
        private Mock<IPedidoService>? mockPedidoService;
        private Mock<IClienteService>? mockClienteService;
        private Mock<ICarrinhoService>? mockCarrinhoService;
        private Mock<IRestauranteService>? mockRestauranteService;
        private Mock<IAvaliacaoService>? mockAvaliacaoService;

        [TestInitialize]
        public void Initialize()
        {
            mockPedidoService = new Mock<IPedidoService>();
            mockClienteService = new Mock<IClienteService>();
            mockCarrinhoService = new Mock<ICarrinhoService>();
            mockRestauranteService = new Mock<IRestauranteService>();
            mockAvaliacaoService = new Mock<IAvaliacaoService>();

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile(new PedidoProfile())).CreateMapper();

            mockPedidoService.Setup(service => service.GetAll()).Returns(GetTestPedidos());
            mockPedidoService.Setup(service => service.Get(1)).Returns(GetTargetPedido());
            mockPedidoService.Setup(service => service.Edit(It.IsAny<Pedido>())).Verifiable();
            mockPedidoService.Setup(service => service.Create(It.IsAny<Pedido>())).Verifiable();
            mockPedidoService.Setup(service => service.Delete(It.IsAny<uint>())).Verifiable();

            controller = new PedidoController(
                mockPedidoService.Object,
                mapper,
                mockClienteService.Object,
                mockCarrinhoService.Object,
                mockRestauranteService.Object,
                mockAvaliacaoService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "teste@email.com"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            var sessionMock = new Mock<ISession>();
            byte[] valueBytes = JsonSerializer.SerializeToUtf8Bytes((uint)1);
            sessionMock.Setup(s => s.TryGetValue("ClienteId", out valueBytes)).Returns(true);

            var httpContext = new DefaultHttpContext { User = user, Session = sessionMock.Object };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        private static PedidoViewModel GetNewPedido()
        {
            return new PedidoViewModel
            {
                Id = 4,
                Status = 'C',
                IdCarrinho = 4,
                IdRestaurante = 4
            };
        }

        private static Pedido GetTargetPedido()
        {
            return new Pedido
            {
                Id = 1,
                Status = 'P',
                IdCarrinho = 1,
                IdRestaurante = 1
            };
        }

        private static PedidoViewModel GetTargetPedidoViewModel()
        {
            return new PedidoViewModel
            {
                Id = 1,
                Status = 'P',
                IdCarrinho = 1,
                IdRestaurante = 1
            };
        }

        private static List<Pedido> GetTestPedidos()
        {
            return new List<Pedido>
            {
                new Pedido
                {
                    Id = 1,
                    Status = 'P',
                    IdCarrinho = 1,
                    IdRestaurante = 1
                },
                new Pedido
                {
                    Id = 2,
                    Status = 'A',
                    IdCarrinho = 2,
                    IdRestaurante = 2
                },
                new Pedido
                {
                    Id = 3,
                    Status = 'E',
                    IdCarrinho = 3,
                    IdRestaurante = 3
                },
            };
        }

        [TestMethod]
        public void IndexTest_Admin_Valido()
        {
            var result = controller!.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(List<PedidoViewModel>));
            List<PedidoViewModel> viewModel = (List<PedidoViewModel>)viewResult.Model;

            Assert.AreEqual(3, viewModel.Count);
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            var result = controller!.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(PedidoViewModel));
            PedidoViewModel viewModel = (PedidoViewModel)viewResult.Model;

            Assert.AreEqual((uint)1, viewModel.Id);
            Assert.AreEqual('P', viewModel.Status);
        }

        [TestMethod]
        public void CreateTest_Get_Valido()
        {
            var result = controller!.Create();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CreateTest_Post_Valid()
        {
            var result = controller!.Create(GetNewPedido());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod]
        public void CreateTest_Post_Invalid()
        {
            controller!.ModelState.AddModelError("Status", "Campo requerido");

            var result = controller.Create(GetNewPedido());

            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult);
        }

        [TestMethod]
        public void DeleteTest_Get_Valid()
        {
            var result = controller!.Delete(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.Model, typeof(PedidoViewModel));
            PedidoViewModel viewModel = (PedidoViewModel)viewResult.Model;

            Assert.AreEqual((uint)1, viewModel.Id);
            Assert.AreEqual('P', viewModel.Status);
        }

        [TestMethod]
        public void DeleteTest_Post_Valid()
        {
            var result = controller!.Delete(GetTargetPedidoViewModel());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }
    }
}