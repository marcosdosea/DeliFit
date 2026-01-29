using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DeliFitWeb.Controllers.Tests;

[TestClass]
public class ItemControllerTests
{

    private ItemController controller = null!;
    private Mock<IItemService> mockService = null!;

    [TestInitialize]
    public void Initialize()
    {
        mockService = new Mock<IItemService>();

        IMapper mapper = new MapperConfiguration(cfg =>

            cfg.AddProfile(new ItemProfile())).CreateMapper();

        mockService.Setup(s => s.GetAll())
            .Returns(GetTestItems());

        //mockService.Setup(s => s.Get(It.IsAny<uint>()))
        //    .Returns(GetTargetItem());

        //Enviezamento necessário para rodar por enquanto que não temos autenticação
        mockService.Setup(s => s.Get(1))
            .Returns(GetTargetItem()); // Retorna apenas para ID = 1

        mockService.Setup(s => s.Get(It.Is<uint>(id => id != 1)))
            .Returns((Item?)null); // Retorna null para outros IDs

        mockService.Setup(s => s.Create(It.IsAny<Item>()));
        mockService.Setup(s => s.Edit(It.IsAny<Item>()));
        mockService.Setup(s => s.Delete(It.IsAny<uint>()));

        controller = new ItemController(mockService.Object, mapper);
    }

    [TestMethod()]
    public void IndexTest_Valido()
    {
        var result = controller.Index(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        ViewResult viewResult = (ViewResult)result;
        var lista = viewResult.Model as List<ItemViewModel>;

        Assert.IsNotNull(lista);
        Assert.HasCount(3, lista);
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
        // Chamar a ação POST que aceita apenas o modelo
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