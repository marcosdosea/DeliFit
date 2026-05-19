using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;

namespace Service.Tests
{
    [TestClass()]
    public class PedidoServiceTests
    {
        private DeliFitContext _context = null!;
        private IPedidoService _pedidoService = null!;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<DeliFitContext>();
            builder.UseInMemoryDatabase("DeliFit");
            var options = builder.Options;

            _context = new DeliFitContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var pedidos = new List<Pedido>
            {
                new()
                {
                    Id = 1,
                    Status = 'P',
                    IdCarrinho = 1,
                    IdRestaurante = 1,
                    Data = new DateTime(2023, 10, 1),
                    Preco = 50.5m
                },
                new()
                {
                    Id = 2,
                    Status = 'A',
                    IdCarrinho = 2,
                    IdRestaurante = 2,
                    Data = new DateTime(2023, 10, 2),
                    Preco = 60.5m
                },
                new()
                {
                    Id = 3,
                    Status = 'E',
                    IdCarrinho = 3,
                    IdRestaurante = 3,
                    Data = new DateTime(2023, 10, 3),
                    Preco = 70.5m
                }
            };

            _context.AddRange(pedidos);
            _context.SaveChanges();

            _pedidoService = new PedidoService(_context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            _pedidoService.Create(new Pedido
            {
                Id = 4,
                Status = 'C',
                IdCarrinho = 4,
                IdRestaurante = 1,
                Data = new DateTime(2023, 10, 4),
                Preco = 80.5m
            });

            // Assert
            Assert.AreEqual(4, _pedidoService.GetAll().Count());

            var pedido = _pedidoService.Get(4);
            Assert.IsNotNull(pedido);
            Assert.AreEqual('C', pedido.Status);
            Assert.AreEqual(4u, pedido.IdCarrinho);
            Assert.AreEqual(80.5m, pedido.Preco);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            _pedidoService.Delete(2);

            // Assert
            Assert.AreEqual(2, _pedidoService.GetAll().Count());
            var pedido = _pedidoService.Get(2);
            Assert.IsNull(pedido);
        }

        [TestMethod()]
        public void EditTest()
        {
            // Act
            var pedido = _pedidoService.Get(3);
            Assert.IsNotNull(pedido);

            pedido.Status = 'C';
            pedido.Preco = 100.0m;

            _pedidoService.Edit(pedido);

            // Assert
            pedido = _pedidoService.Get(3);
            Assert.IsNotNull(pedido);
            Assert.AreEqual('C', pedido.Status);
            Assert.AreEqual(100.0m, pedido.Preco);
        }

        [TestMethod()]
        public void GetTest()
        {
            var pedido = _pedidoService.Get(1);

            Assert.IsNotNull(pedido);
            Assert.AreEqual('P', pedido.Status);
            Assert.AreEqual(1u, pedido.IdCarrinho);
            Assert.AreEqual(50.5m, pedido.Preco);
            Assert.AreEqual(new DateTime(2023, 10, 1), pedido.Data);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaPedidos = _pedidoService.GetAll();

            // Assert
            Assert.IsNotNull(listaPedidos);
            Assert.AreEqual(3, listaPedidos.Count());

            var primeiro = listaPedidos.First();
            Assert.AreEqual((uint)1, primeiro.Id);
            Assert.AreEqual('P', primeiro.Status);
            Assert.AreEqual(50.5m, primeiro.Preco);
        }
    }
}