using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Service.Tests
{
    [TestClass]
    public class CartaoServiceTests
    {
        private DeliFitContext context;
        private ICartaoService cartaoService;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<DeliFitContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new DeliFitContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Cartaos.AddRange(
                new Cartao
                {
                    Id = 1,
                    Nome = "Kauan Brilhante",
                    Cpf = "11111111111",
                    Numero = "1111222233334444",
                    Validade = new DateTime(2027, 12, 1),
                    Cvv = "123",
                    IdCliente = 1
                },
                new Cartao
                {
                    Id = 2,
                    Nome = "Kauan Brilhante",
                    Cpf = "22222222222",
                    Numero = "5555666677778888",
                    Validade = new DateTime(2028, 6, 1),
                    Cvv = "456",
                    IdCliente = 1
                },
                new Cartao
                {
                    Id = 3,
                    Nome = "Outro Nome",
                    Cpf = "33333333333",
                    Numero = "9999000011112222",
                    Validade = new DateTime(2029, 1, 1),
                    Cvv = "789",
                    IdCliente = 1
                }
        );


            context.SaveChanges();

            cartaoService = new CartaoService(context);
        }

        [TestMethod]
        public void CreateTest()
        {
            cartaoService.Create(new Cartao
            {
                Id = 4,
                Nome = "Novo Cartao",
                Cpf = "12312312312",
                Numero = "1234123412341234",
                Validade = new DateTime(2030, 5, 1),
                Cvv = "321",
                IdCliente = 1
            });

            Assert.AreEqual(4, cartaoService.GetAll().Count());

            var cartao = cartaoService.Get(4);
            Assert.IsNotNull(cartao);
            Assert.AreEqual("Novo Cartao", cartao.Nome);
        }

        [TestMethod]
        public void DeleteTest()
        {
            cartaoService.Delete(2);

            Assert.AreEqual(2, cartaoService.GetAll().Count());
            Assert.IsNull(cartaoService.Get(2));
        }

        [TestMethod]
        public void GetTest()
        {
            var cartao = cartaoService.Get(1);

            Assert.IsNotNull(cartao);
            Assert.AreEqual("Kauan Brilhante", cartao.Nome);
            Assert.AreEqual("1111222233334444", cartao.Numero);
            Assert.AreEqual("11111111111", cartao.Cpf);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var lista = cartaoService.GetAll();

            Assert.IsNotNull(lista);
            Assert.AreEqual(3, lista.Count());

            var primeiro = lista.First();
            Assert.AreEqual((uint)1, primeiro.Id);
            Assert.AreEqual("Kauan Brilhante", primeiro.Nome);
        }
    }
}
