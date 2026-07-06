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
    public class ItemControllerTests
    {
        private ItemController controller = null!;
        private Mock<IItemService> mockService = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockService = new Mock<IItemService>();
            var mockRestauranteService = new Mock<IRestauranteService>();
            var mockCategoriaService = new Mock<ICategoriaService>();
            mockCategoriaService.Setup(s => s.ListarCategorias()).Returns(new List<Core.DTO.CategoriaDTO>());

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new ItemProfile())).CreateMapper();

            mockService.Setup(s => s.GetAll())
                .Returns(GetTestItems());

            mockService.Setup(s => s.GetByRestaurante(It.IsAny<uint>()))
                .Returns(GetTestItems());

            mockService.Setup(s => s.Get(1))
                .Returns(GetTargetItem());

            mockService.Setup(s => s.Get(It.Is<uint>(id => id != 1)))
                .Returns((Item?)null);

            mockService.Setup(s => s.Create(It.IsAny<Item>()));
            mockService.Setup(s => s.Edit(It.IsAny<Item>()));
            mockService.Setup(s => s.Delete(It.IsAny<uint>()));

            controller = new ItemController(mockService.Object, mapper, mockRestauranteService.Object, mockCategoriaService.Object);

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

        [TestMethod()]
        public void IndexTest_Valido()
        {
            var result = controller.Index(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            var lista = viewResult.Model as List<ItemViewModel>;

            Assert.IsNotNull(lista);
            Assert.AreEqual(3, lista.Count);
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            var result = controller?.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var model = ((ViewResult)result).Model as ItemViewModel;

            Assert.AreEqual("Bife à Parmegiana", model.Nome);
            Assert.AreEqual(350f, model.Calorias);
            Assert.AreEqual(29.99m, model.Preco);
            Assert.AreEqual("Contém Glúten", model.Restricao);
        }

        [TestMethod()]
        public void CreateTest_Get_Valido()
        {
            uint idRestaurante = 1;

            var result = controller?.Create(idRestaurante);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void CreateTest_Post_Valid()
        {
            var result = controller.Create(GetNovoItem());

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod()]
        public void CreateTest_Post_Invalid()
        {
            controller?.ModelState.AddModelError("Nome", "Nome é obrigatório.");

            var result = controller?.Create(GetNovoItem());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void EditTest_Get_Valid()
        {
            var result = controller.Edit(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void EditTest_Post_Valid()
        {
            var model = GetTargetItemViewModel();
            var result = controller.Edit(model);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod()]
        public void DeleteTest_Get_Valid()
        {
            var result = controller.Delete(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void DeleteTest_Post_Valid()
        {
            var model = GetTargetItemViewModel();
            var result = controller.Delete(model.Id, model);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        private ItemViewModel GetNovoItem()
        {
            return new ItemViewModel
            {
                Id = 4,
                Nome = "Salmão Grelhado",
                Calorias = 280,
                Carboidratos = 5,
                Gordura = 12,
                Proteina = 35,
                Restricao = "Sem Glúten",
                Descricao = "Salmão grelhado com ervas finas",
                Preco = 42.50m,
                Tamanho = "Grande",
                Volume = null,
                IdRestaurante = 1
            };
        }

        private static Item GetTargetItem()
        {
            return new Item
            {
                Id = 1,
                Nome = "Bife à Parmegiana",
                Calorias = 350,
                Carboidratos = 25,
                Gordura = 15,
                Proteina = 30,
                Restricao = "Contém Glúten",
                Descricao = "Bife à parmegiana com arroz e fritas",
                Preco = 29.99m,
                Tamanho = "Médio",
                Volume = null,
                IdRestaurante = 1
            };
        }

        private ItemViewModel GetTargetItemViewModel()
        {
            return new ItemViewModel
            {
                Id = 1,
                Nome = "Bife à Parmegiana",
                Calorias = 350,
                Carboidratos = 25,
                Gordura = 15,
                Proteina = 30,
                Restricao = "Contém Glúten",
                Descricao = "Bife à parmegiana com arroz e fritas",
                Preco = 29.99m,
                Tamanho = "Médio",
                Volume = null,
                IdRestaurante = 1
            };
        }

        private IEnumerable<Item> GetTestItems()
        {
            return new List<Item>
            {
                new Item
                {
                    Id = 1,
                    Nome = "Bife à Parmegiana",
                    Calorias = 350,
                    Carboidratos = 25,
                    Gordura = 15,
                    Proteina = 30,
                    Restricao = "Contém Glúten",
                    Descricao = "Bife à parmegiana com arroz e fritas",
                    Preco = 29.99m,
                    Tamanho = "Médio",
                    Volume = null,
                    IdRestaurante = 1
                },
                new Item
                {
                    Id = 2,
                    Nome = "Salada Caesar",
                    Calorias = 220,
                    Carboidratos = 15,
                    Gordura = 8,
                    Proteina = 12,
                    Restricao = "Sem Lactose",
                    Descricao = "Salada Caesar com frango grelhado",
                    Preco = 24.90m,
                    Tamanho = "Grande",
                    Volume = null,
                    IdRestaurante = 1
                },
                new Item
                {
                    Id = 3,
                    Nome = "Suco de Laranja Natural",
                    Calorias = 120,
                    Carboidratos = 28,
                    Gordura = 0,
                    Proteina = 2,
                    Restricao = "Vegano",
                    Descricao = "Suco de laranja natural 500ml",
                    Preco = 8.50m,
                    Tamanho = null,
                    Volume = "500ml",
                    IdRestaurante = 1
                }
            };
        }
    }
}