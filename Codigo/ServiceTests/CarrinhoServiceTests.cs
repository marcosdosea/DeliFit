using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace ServiceTests
{
    [TestClass()]
    public class CarrinhoServiceTests
    {
        private DeliFitContext _context;
        private ICarrinhoService _carrinhoService;

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

            var carrinhos = new List<Carrinho>
            {
                new()
                {
                    Id = 1,
                    Observacao = "Carne ao ponto",
                    IdCliente = 2,
                    FormaDePagamento = "D",
                    ValorFrete = 10.50M,
                    IdCartao = null
                },
                new()
                {
                    Id = 2,
                    Observacao = "Sem cebola",
                    IdCliente = 2,
                    FormaDePagamento = "C",
                    ValorFrete = 10.50M,
                    IdCartao = 1
                },
                new()
                {
                    Id = 3,
                    Observacao = "Vegano",
                    IdCliente = 3,
                    FormaDePagamento = "P",
                    ValorFrete = 8.00M,
                    IdCartao = null
                }
            };

            _context.AddRange(carrinhos);
            _context.SaveChanges();

            _carrinhoService = new CarrinhoService(_context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            _carrinhoService.Create(new Carrinho
            {
                Id = 4,
                Observacao = "Atecubanos",
                IdCliente = 2,
                FormaDePagamento = "P",
                ValorFrete = 10.00M,
                IdCartao = null
            });

            // Assert
            Assert.AreEqual(4, _carrinhoService.GetAll().Count());

            var cliente = _carrinhoService.Get(4);
            Assert.IsNotNull(cliente);
            Assert.AreEqual("P", cliente.FormaDePagamento);
            Assert.AreEqual("Atecubanos", cliente.Observacao);

        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            _carrinhoService.Delete(2);

            // Assert
            Assert.AreEqual(2, _carrinhoService.GetAll().Count());
            var cliente = _carrinhoService.Get(2);
            Assert.IsNull(cliente);
        }




        [TestMethod()]
        public void GetTest()
        {
            var carrinho = _carrinhoService.Get(1);

            Assert.IsNotNull(carrinho);
            Assert.AreEqual("Carne ao ponto", carrinho.Observacao);
            Assert.AreEqual("D", carrinho.FormaDePagamento);

        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaCarrinho = _carrinhoService.GetAll();

            // Assert
            Assert.IsNotNull(listaCarrinho);
            Assert.AreEqual(3, listaCarrinho.Count());

            var primeiro = listaCarrinho.First();
            Assert.AreEqual((uint)1, primeiro.Id);
            Assert.AreEqual("Carne ao ponto", primeiro.Observacao);
            Assert.AreEqual("D", primeiro.FormaDePagamento);
        }
    }
}
