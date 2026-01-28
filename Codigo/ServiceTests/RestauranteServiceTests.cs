
using Core;
using Core.Service;
using Core.DTO;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass]
    public class RestauranteServiceTests
    {
        private DeliFitContext? context;
        private IRestauranteService? restauranteService;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<DeliFitContext>();
            builder.UseInMemoryDatabase("DeliFitDBTest");
            var options = builder.Options;

            context = new DeliFitContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var restaurantes = new List<Restaurante>
                {
                    new() { Id = 1, NomeRestaurante = "Restaurante 1", Cidade = "Cidade 1",  Bairro = "Bairro", Cep = "12345-789",
                Cnpj = "12345678910121", CpfProprietario = "12345678901", Email = "teste@gmail.com", Estado = "Estado", Numero = "123", Rua = "Rua",
                TelefoneProprietario = "79999419916", TelefoneRestaurante = "79999419916", NomeProprietario = "NomeTeste"},
                    new Restaurante { Id = 2, NomeRestaurante = "Restaurante 2", Cidade = "Cidade 2",  Bairro = "Bairro", Cep = "12345-789",
                Cnpj = "12345678910122", CpfProprietario = "12345678902", Email = "teste2@gmail.com", Estado = "Estado", Numero = "123", Rua = "Rua",
                TelefoneProprietario = "79999419916", TelefoneRestaurante = "79999419916", NomeProprietario = "NomeTeste2"},
                    new Restaurante { Id = 3, NomeRestaurante = "Restaurante 3", Cidade = "Cidade 3",  Bairro = "Bairro", Cep = "12345-789",
                Cnpj = "12345678910123", CpfProprietario = "12345678903", Email = "teste3@gmail.com", Estado = "Estado", Numero = "123", Rua = "Rua",
                TelefoneProprietario = "79999419916", TelefoneRestaurante = "79999419916", NomeProprietario = "NomeTeste3"},
                };

            context.AddRange(restaurantes);
            context.SaveChanges();

            restauranteService = new RestauranteService(context);
        }

        [TestMethod]
        public void CreateTest()
        {
            // Act
            restauranteService?.Create(new Restaurante() { Id = 4, NomeRestaurante = "Restaurante 4", Cidade = "Cidade 4", Bairro = "Bairro", Cep = "12345-789",
                Cnpj = "12345678910124", CpfProprietario = "12345678904", Email = "teste4@gmail.com", Estado = "Estado", Numero = "123", Rua = "Rua",
                TelefoneProprietario = "79999419916", TelefoneRestaurante = "79999419916", NomeProprietario = "NomeTeste4"});
            // Assert
            Assert.AreEqual(4, restauranteService?.GetAll().Count());
            var restaurante = restauranteService?.Get(4);
            Assert.AreEqual("Restaurante 4", restaurante!.NomeRestaurante);
            Assert.AreEqual("Cidade 4", restaurante!.Cidade);
        }

        [TestMethod]
        public void DeleteTest()
        {
            // Act
            restauranteService?.Delete(2);
            // Assert
            Assert.AreEqual(2, restauranteService?.GetAll().Count());
            var restaurante = restauranteService?.Get(2);
            Assert.IsNull(restaurante);
        }

        [TestMethod]
        public void EditTest()
        {
            //Act 
            var restaurante = restauranteService?.Get(3);
            restaurante!.NomeProprietario = "Restaurante 3";
            restaurante!.Cidade = "Cidade 3";
            restauranteService?.Edit(restaurante);
            //Assert
            restaurante = restauranteService?.Get(3);
            Assert.IsNotNull(restaurante);
            Assert.AreEqual("Restaurante 3", restaurante.NomeRestaurante);
            Assert.AreEqual("Cidade 3", restaurante.Cidade);
        }

        [TestMethod]
        public void GetTest()
        {
            var restaurante = restauranteService?.Get(1);
            Assert.IsNotNull(restaurante);
            Assert.AreEqual("Restaurante 1", restaurante.NomeRestaurante);
            Assert.AreEqual("Cidade 1", restaurante.Cidade);
        }

        [TestMethod]
        public void GetAllTest()
        {
            // Act
            var listaRestaurante = restauranteService?.GetAll();
            // Assert
            Assert.IsInstanceOfType(listaRestaurante, typeof(IEnumerable<RestauranteDTO>));
            Assert.IsNotNull(listaRestaurante);
            Assert.AreEqual(3, listaRestaurante.Count());
            Assert.AreEqual((uint)1, listaRestaurante.First().Id);
            Assert.AreEqual("Restaurante 1", listaRestaurante.First().NomeRestaurante);
        }
    }
}