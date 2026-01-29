using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Service.Tests
{
    [TestClass]
    public class EnderecoServiceTests
    {
        private DeliFitContext context;
        private IEnderecoService enderecoService;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<DeliFitContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new DeliFitContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Enderecos.AddRange(
                new Endereco
                {
                    Id = 1,
                    Rua = "Rua A",
                    Numero = "10",
                    Bairro = "Bairro A",
                    Cidade = "Aracaju",
                    Estado = "SE",
                    Cep = "49000000",
                    Label = "Casa",
                    IdCliente = 1
                },
                new Endereco
                {
                    Id = 2,
                    Rua = "Rua B",
                    Numero = "20",
                    Bairro = "Bairro B",
                    Cidade = "Aracaju",
                    Estado = "SE",
                    Cep = "49000001",
                    Label = "Trabalho",
                    IdCliente = 1
                },
                new Endereco
                {
                    Id = 3,
                    Rua = "Rua C",
                    Numero = "30",
                    Bairro = "Bairro C",
                    Cidade = "Aracaju",
                    Estado = "SE",
                    Cep = "49000002",
                    Label = "Outro",
                    IdCliente = 1
                }
            );

            context.SaveChanges();

            enderecoService = new EnderecoService(context);
        }

        [TestMethod]
        public void CreateTest()
        {
            enderecoService.Create(new Endereco
            {
                Id = 4,
                Rua = "Rua D",
                Numero = "40",
                Bairro = "Bairro D",
                Cidade = "Aracaju",
                Estado = "SE",
                Cep = "49000003",
                Label = "Casa",
                IdCliente = 1
            });

            Assert.AreEqual(4, enderecoService.GetAll().Count());

            var endereco = enderecoService.Get(4);
            Assert.IsNotNull(endereco);
            Assert.AreEqual("Rua D", endereco.Rua);
        }

        [TestMethod]
        public void DeleteTest()
        {
            enderecoService.Delete(2);

            Assert.AreEqual(2, enderecoService.GetAll().Count());
            Assert.IsNull(enderecoService.Get(2));
        }

        [TestMethod]
        public void EditTest()
        {
            var endereco = enderecoService.Get(3);

            endereco.Rua = "Rua Alterada";
            endereco.Numero = "99";

            enderecoService.Edit(endereco);

            endereco = enderecoService.Get(3);

            Assert.IsNotNull(endereco);
            Assert.AreEqual("Rua Alterada", endereco.Rua);
            Assert.AreEqual("99", endereco.Numero);
        }

        [TestMethod]
        public void GetTest()
        {
            var endereco = enderecoService.Get(1);

            Assert.IsNotNull(endereco);
            Assert.AreEqual("Rua A", endereco.Rua);
            Assert.AreEqual("10", endereco.Numero);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var lista = enderecoService.GetAll();

            Assert.IsNotNull(lista);
            Assert.AreEqual(3, lista.Count());

            var primeiro = lista.First();
            Assert.AreEqual((uint)1, primeiro.Id);
            Assert.AreEqual("Rua A", primeiro.Rua);
        }
    }
}
