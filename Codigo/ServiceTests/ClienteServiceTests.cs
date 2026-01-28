using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass()]
    public class ClienteServiceTests
    {
        private DeliFitContext _context;
        private IClienteService _clienteService;

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

            var clientes = new List<Cliente>
            {
                new()
                {
                    Id = 1,
                    Nome = "Machado de Assis",
                    Telefone = "79999999999",
                    Email = "machado@exemplo.com",
                    Cpf = "11111111111"
                },
                new()
                {
                    Id = 2,
                    Nome = "Ian S. Sommerville",
                    Telefone = "79988888888",
                    Email = "ian@exemplo.com",
                    Cpf = "22222222222"
                },
                new()
                {
                    Id = 3,
                    Nome = "Gleford Myers",
                    Telefone = "79977777777",
                    Email = "gleford@exemplo.com",
                    Cpf = "33333333333"
                }
            };

            _context.AddRange(clientes);
            _context.SaveChanges();

            _clienteService = new ClienteService(_context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            _clienteService.Create(new Cliente
            {
                Id = 4,
                Nome = "Graciliano Ramos",
                Telefone = "79966666666",
                Email = "graciliano@exemplo.com",
                Cpf = "44444444444"
            });

            // Assert
            Assert.AreEqual(4, _clienteService.GetAll().Count());

            var cliente = _clienteService.Get(4);
            Assert.IsNotNull(cliente);
            Assert.AreEqual("Graciliano Ramos", cliente.Nome);
            Assert.AreEqual("79966666666", cliente.Telefone);
            Assert.AreEqual("graciliano@exemplo.com", cliente.Email);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            _clienteService.Delete(2);

            // Assert
            Assert.AreEqual(2, _clienteService.GetAll().Count());
            var cliente = _clienteService.Get(2);
            Assert.IsNull(cliente);
        }

        [TestMethod()]
        public void EditTest()
        {
            // Act
            var cliente = _clienteService.Get(3);
            Assert.IsNotNull(cliente);

            cliente.Nome = "Paulo Coelho";
            cliente.Telefone = "79955555555";
            cliente.Email = "paulo@exemplo.com";

            _clienteService.Edit(cliente);

            // Assert
            cliente = _clienteService.Get(3);
            Assert.IsNotNull(cliente);
            Assert.AreEqual("Paulo Coelho", cliente.Nome);
            Assert.AreEqual("79955555555", cliente.Telefone);
            Assert.AreEqual("paulo@exemplo.com", cliente.Email);
        }

        [TestMethod()]
        public void GetTest()
        {
            var cliente = _clienteService.Get(1);

            Assert.IsNotNull(cliente);
            Assert.AreEqual("Machado de Assis", cliente.Nome);
            Assert.AreEqual("79999999999", cliente.Telefone);
            Assert.AreEqual("machado@exemplo.com", cliente.Email);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaCliente = _clienteService.GetAll();

            // Assert
            Assert.IsNotNull(listaCliente);
            Assert.AreEqual(3, listaCliente.Count());

            var primeiro = listaCliente.First();
            Assert.AreEqual((uint)1, primeiro.Id);
            Assert.AreEqual("Machado de Assis", primeiro.Nome);
            Assert.AreEqual("machado@exemplo.com", primeiro.Email);
        }
    }
}
