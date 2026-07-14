using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace ServiceTests;

[TestClass]
public class AdesaoServiceTests
{
    private DeliFitContext? context;
    private IRestauranteService? restauranteService;

    [TestInitialize]
    public void Initialize()
    {
        var builder = new DbContextOptionsBuilder<DeliFitContext>();
        builder.UseInMemoryDatabase("DeliFitAdesaoTest");
        var options = builder.Options;

        context = new DeliFitContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var restaurantes = new List<Restaurante>
        {
            new() {
                Id = 1,
                NomeRestaurante = "Restaurante Pendente 1",
                Cidade = "Cidade 1",
                Bairro = "Bairro",
                Cep = "12345789",
                Cnpj = "12345678910121",
                CpfProprietario = "12345678901",
                Email = "teste1@gmail.com",
                Estado = "SP",
                Numero = "123",
                Rua = "Rua Teste",
                TelefoneProprietario = "79999419916",
                TelefoneRestaurante = "79999419916",
                NomeProprietario = "NomeTeste1",
                Validado = false,
                Descricao = "Restaurante pendente de aprovação"
            },
            new() {
                Id = 2,
                NomeRestaurante = "Restaurante Aprovado 1",
                Cidade = "Cidade 2",
                Bairro = "Bairro",
                Cep = "12345789",
                Cnpj = "12345678910122",
                CpfProprietario = "12345678902",
                Email = "teste2@gmail.com",
                Estado = "RJ",
                Numero = "456",
                Rua = "Rua Aprovada",
                TelefoneProprietario = "79999419917",
                TelefoneRestaurante = "79999419917",
                NomeProprietario = "NomeTeste2",
                Validado = true,
                Descricao = "Restaurante já aprovado"
            },
            new() {
                Id = 3,
                NomeRestaurante = "Restaurante Pendente 2",
                Cidade = "Cidade 3",
                Bairro = "Bairro",
                Cep = "12345789",
                Cnpj = "12345678910123",
                CpfProprietario = "12345678903",
                Email = "teste3@gmail.com",
                Estado = "MG",
                Numero = "789",
                Rua = "Rua Pendente",
                TelefoneProprietario = "79999419918",
                TelefoneRestaurante = "79999419918",
                NomeProprietario = "NomeTeste3",
                Validado = false,
                Descricao = "Outro restaurante pendente"
            }
        };

        context.AddRange(restaurantes);
        context.SaveChanges();

        restauranteService = new RestauranteService(context);
    }

    [TestMethod]
    public void AprovarSolicitacaoTest()
    {
        var restaurantePendente = restauranteService?.Get(1);

        restaurantePendente!.Validado = true;
        restauranteService?.Edit(restaurantePendente);

        var restauranteAprovado = restauranteService?.Get(1);
        Assert.IsNotNull(restauranteAprovado);
        Assert.AreEqual(true, restauranteAprovado.Validado);
        Assert.AreEqual("Restaurante Pendente 1", restauranteAprovado.NomeRestaurante);

        var listaAprovados = restauranteService?.GetAll()
            .Where(r => r.Validado == true)
            .ToList();
        Assert.IsNotNull(listaAprovados);
        Assert.AreEqual(2, listaAprovados.Count);
        Assert.IsTrue(listaAprovados.Any(r => r.Id == 1));
    }

    [TestMethod]
    public void NegarSolicitacaoTest()
    {
        var restaurantePendente = restauranteService?.Get(3);
        Assert.IsNotNull(restaurantePendente);
        Assert.AreEqual(false, restaurantePendente.Validado);

        restauranteService?.Delete(3);

        var restauranteExcluido = restauranteService?.Get(3);
        Assert.IsNull(restauranteExcluido);

        var listaPendentes = restauranteService?.GetAll()
            .Where(r => r.Validado == false)
            .ToList();
        Assert.IsNotNull(listaPendentes);
        Assert.AreEqual(1, listaPendentes.Count);
        Assert.IsFalse(listaPendentes.Any(r => r.Id == 3));
    }

    [TestMethod]
    public void ListarSolicitacoesPendentesTest()
    {
        var listaPendentes = restauranteService?.GetAll()
            .Where(r => r.Validado == false)
            .ToList();

        Assert.IsNotNull(listaPendentes);
        Assert.AreEqual(2, listaPendentes.Count);
        Assert.IsTrue(listaPendentes.All(r => r.Validado == false));
        Assert.AreEqual("Restaurante Pendente 1", listaPendentes[0].NomeRestaurante);
        Assert.AreEqual("Restaurante Pendente 2", listaPendentes[1].NomeRestaurante);
    }

    [TestMethod]
    public void ListarRestaurantesAtivosTest()
    {
        var listaAtivos = restauranteService?.GetAll()
            .Where(r => r.Validado == true)
            .ToList();

        Assert.IsNotNull(listaAtivos);
        Assert.AreEqual(1, listaAtivos.Count);
        Assert.IsTrue(listaAtivos.All(r => r.Validado == true));
        Assert.AreEqual("Restaurante Aprovado 1", listaAtivos[0].NomeRestaurante);
    }

    [TestMethod]
    public void CriarSolicitacaoAdesaoTest()
    {
        var novoRestaurante = new Restaurante()
        {
            Id = 4,
            NomeRestaurante = "Novo Restaurante",
            Cidade = "Cidade Nova",
            Bairro = "Bairro Novo",
            Cep = "54321987",
            Cnpj = "98765432109876",
            CpfProprietario = "98765432109",
            Email = "novo@gmail.com",
            Estado = "PR",
            Numero = "999",
            Rua = "Rua Nova",
            TelefoneProprietario = "79999419919",
            TelefoneRestaurante = "79999419919",
            NomeProprietario = "Novo Proprietario",
            Validado = false,
            Descricao = "Novo restaurante solicitando adesão"
        };

        restauranteService?.Create(novoRestaurante);

        Assert.AreEqual(4, restauranteService?.GetAll().Count());

        var restauranteCriado = restauranteService?.Get(4);
        Assert.IsNotNull(restauranteCriado);
        Assert.AreEqual("Novo Restaurante", restauranteCriado.NomeRestaurante);
        Assert.AreEqual(false, restauranteCriado.Validado);

        var listaPendentes = restauranteService?.GetAll()
            .Where(r => r.Validado == false)
            .ToList();
        Assert.IsNotNull(listaPendentes);
        Assert.AreEqual(3, listaPendentes.Count);
        Assert.IsTrue(listaPendentes.Any(r => r.Id == 4));
    }
}