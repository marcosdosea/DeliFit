using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class ItemServiceTests
{
    private DeliFitContext _context;
    private IItemService _itemService;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<DeliFitContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        _context = new DeliFitContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _context.Items.AddRange(
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
        );

        _context.SaveChanges();

        _itemService = new ItemService(_context);
    }

    [TestMethod()]
    public void CreateTest()
    {
        _itemService.Create(new Item
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
        });

        Assert.AreEqual(4, _itemService.GetAll().Count());

        var item = _itemService.Get(4);
        Assert.IsNotNull(item);
        Assert.AreEqual("Salmão Grelhado", item.Nome);
        Assert.AreEqual(280f, item.Calorias);
        Assert.AreEqual(42.50m, item.Preco);
        Assert.AreEqual("Sem Glúten", item.Restricao);
    }

    [TestMethod()]
    public void DeleteTest()
    {
        _itemService.Delete(2);

        Assert.AreEqual(2, _itemService.GetAll().Count());
        Assert.IsNull(_itemService.Get(2));
    }

    [TestMethod()]
    public void EditTest()
    {
        var item = _itemService.Get(3);

        item.Nome = "Suco de Laranja Natural 1L";
        item.Calorias = 240;
        item.Preco = 12.00m;
        item.Volume = "1L";

        _itemService.Edit(item);

        item = _itemService.Get(3);
        Assert.IsNotNull(item);
        Assert.AreEqual("Suco de Laranja Natural 1L", item.Nome);
        Assert.AreEqual(240f, item.Calorias);
        Assert.AreEqual(12.00m, item.Preco);
        Assert.AreEqual("1L", item.Volume);
    }

    [TestMethod()]
    public void GetTest()
    {
        var item = _itemService.Get(1);

        Assert.IsNotNull(item);
        Assert.AreEqual("Bife à Parmegiana", item.Nome);
        Assert.AreEqual(350f, item.Calorias);
        Assert.AreEqual(29.99m, item.Preco);
        Assert.AreEqual("Contém Glúten", item.Restricao);
        Assert.AreEqual(25f, item.Carboidratos);
        Assert.AreEqual(15f, item.Gordura);
        Assert.AreEqual(30f, item.Proteina);
    }

    [TestMethod()]
    public void GetAllTest()
    {
        var listaItens = _itemService.GetAll();

        Assert.IsNotNull(listaItens);
        Assert.AreEqual(3, listaItens.Count());

        var primeiro = listaItens.First();
        Assert.AreEqual((uint)1, primeiro.Id);
        Assert.AreEqual("Bife à Parmegiana", primeiro.Nome);
        Assert.AreEqual(29.99m, primeiro.Preco);
    }
}
