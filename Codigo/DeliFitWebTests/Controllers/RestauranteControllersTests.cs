using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using DeliFitWeb.Controllers;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace DeliFitWebTests.Controllers
{
    [TestClass]
    public class RestauranteControllersTests
    {
        // 1. Definição dos campos globais (arquitetura do Código 1)
        private Mock<IRestauranteService>? _serviceMock;
        private IMapper? _mapper;
        private RestauranteController? _controller;

        // 2. Método de Inicialização (roda antes de CADA teste)
        [TestInitialize]
        public void Initialize()
        {
            // Configuração do Mapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RestauranteProfile>());
            _mapper = config.CreateMapper();

            // Configuração do Mock do Serviço
            _serviceMock = new Mock<IRestauranteService>();

            // Instancia o Controller (SUT - Subject Under Test)
            // O Controller já é criado com o Mock e o Mapper injetados
            _controller = new RestauranteController(_serviceMock.Object, _mapper);
        }

        [TestMethod]
        public void Index_ReturnsViewWithMappedModels()
        {
            // Arrange (Configurar o comportamento do Mock para este teste específico)
            var dtoList = new List<RestauranteDTO>
            {
                new RestauranteDTO { Id = 1, NomeRestaurante = "R1", Validado = true, Cidade = "C1", Estado = "E1" }
            };
            _serviceMock!.Setup(s => s.GetAll()).Returns(dtoList);

            // Act
            var result = _controller!.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(List<RestauranteViewModel>));
            var model = (List<RestauranteViewModel>)viewResult.Model;

            Assert.HasCount(1, model);
            Assert.AreEqual("R1", model[0].NomeRestaurante);
            Assert.AreEqual("C1", model[0].Cidade);
            Assert.AreEqual("E1", model[0].Estado);
            Assert.IsTrue(model[0].Validado);
        }

        [TestMethod]
        public void Details_ReturnsViewWithMappedModel()
        {
            // Arrange
            var restaurante = new Restaurante { Id = 2, NomeRestaurante = "R2", Validado = false, Cidade = "C2", Estado = "E2" };
            _serviceMock!.Setup(s => s.Get(2)).Returns(restaurante);

            // Act
            var result = _controller!.Details(2);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(RestauranteViewModel));
            var model = (RestauranteViewModel)viewResult.Model;

            Assert.AreEqual((uint)2, model.Id);
            Assert.AreEqual("R2", model.NomeRestaurante);
            Assert.IsFalse(model.Validado);
            Assert.AreEqual("C2", model.Cidade);
            Assert.AreEqual("E2", model.Estado);
        }

        [TestMethod]
        public void Create_Post_ValidModel_CallsCreateAndRedirects()
        {
            // Arrange
            var vm = new RestauranteViewModel { Id = 3, NomeRestaurante = "R3", Validado = true, Cidade = "C3", Estado = "E3" };

            // Act
            var result = _controller!.Create(vm);

            // Assert
            // Verificamos se o método Create do serviço foi chamado corretamente
            _serviceMock!.Verify(s => s.Create(It.Is<Restaurante>(r => r.NomeRestaurante == vm.NomeRestaurante && r.Cidade == vm.Cidade)), Times.Once);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;

            Assert.AreEqual(nameof(RestauranteController.Index), redirect.ActionName);
        }

        [TestMethod]
        public void Create_Post_InvalidModel_DoesNotCallCreateAndRedirects()
        {
            // Arrange
            var vm = new RestauranteViewModel { Id = 4, NomeRestaurante = "R4" };
            // Simulamos um erro de validação no ModelState do controller
            _controller!.ModelState.AddModelError("NomeRestaurante", "Required");

            // Act
            var result = _controller.Create(vm);

            // Assert
            _serviceMock!.Verify(s => s.Create(It.IsAny<Restaurante>()), Times.Never);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;

            Assert.AreEqual(nameof(RestauranteController.Index), redirect.ActionName);
        }

        [TestMethod]
        public void Edit_Get_ReturnsViewWithMappedModel()
        {
            // Arrange
            var restaurante = new Restaurante { Id = 5, NomeRestaurante = "R5", Validado = true, Cidade = "C5", Estado = "E5" };
            _serviceMock!.Setup(s => s.Get(5)).Returns(restaurante);

            // Act
            var result = _controller!.Edit(5);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(RestauranteViewModel));
            var model = (RestauranteViewModel)viewResult.Model;

            Assert.AreEqual((uint)5, model.Id);
            Assert.AreEqual("R5", model.NomeRestaurante);
        }

        [TestMethod]
        public void Edit_Post_ValidModel_CallsEditAndRedirects()
        {
            // Arrange
            var vm = new RestauranteViewModel { Id = 6, NomeRestaurante = "R6", Cidade = "C6", Estado = "E6" };

            // Act
            var result = _controller!.Edit(vm);

            // Assert
            _serviceMock!.Verify(s => s.Edit(It.Is<Restaurante>(r => r.NomeRestaurante == vm.NomeRestaurante && r.Id == vm.Id)), Times.Once);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;

            Assert.AreEqual(nameof(RestauranteController.Index), redirect.ActionName);
        }

        [TestMethod]
        public void Delete_Get_ReturnsViewWithMappedModel()
        {
            // Arrange
            var restaurante = new Restaurante { Id = 7, NomeRestaurante = "R7", Validado = false, Cidade = "C7", Estado = "E7" };
            _serviceMock!.Setup(s => s.Get(7)).Returns(restaurante);

            // Act
            var result = _controller!.Delete(7);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(RestauranteViewModel));
            var model = (RestauranteViewModel)viewResult.Model;

            Assert.AreEqual((uint)7, model.Id);
            Assert.AreEqual("R7", model.NomeRestaurante);
        }

        [TestMethod]
        public void Delete_Post_CallsDeleteAndRedirects()
        {
            // Arrange
            var vm = new RestauranteViewModel { Id = 8, NomeRestaurante = "R8" };

            // Act
            var result = _controller!.Delete(8, vm);

            // Assert
            _serviceMock!.Verify(s => s.Delete(8), Times.Once);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;

            Assert.AreEqual(nameof(RestauranteController.Index), redirect.ActionName);
        }
    }
}