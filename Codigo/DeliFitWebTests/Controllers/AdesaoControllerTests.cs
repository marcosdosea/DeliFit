using AutoMapper;
using Core;
using Core.DTO;
using Core.Identity.Data;
using Core.Service;
using DeliFitWeb.Controllers;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace DeliFitWebTests.Controllers;

[TestClass]
public class AdesaoControllerTests
{
    private RestauranteController? controller;
    private Mock<IRestauranteService>? mockService;
    private Mock<UserManager<UsuarioIdentity>>? mockUserManager;

    [TestInitialize]
    public void Initialize()
    {
        mockService = new Mock<IRestauranteService>();

        mockUserManager = new Mock<UserManager<UsuarioIdentity>>(
            Mock.Of<IUserStore<UsuarioIdentity>>(), null, null, null, null, null, null, null, null);

        var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

        var mockEmailSender = new Mock<IEmailSender>();

        // Configura UserManager para simular criação de usuário com sucesso
        mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((UsuarioIdentity?)null);
        mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UsuarioIdentity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<UsuarioIdentity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile(new RestauranteProfile())).CreateMapper();

        mockService.Setup(service => service.GetAll()).Returns(() => GetTestRestaurantesCompletosDTO());
        mockService.Setup(service => service.GetRestaurantesAtivos()).Returns(() => GetTestRestaurantesCompletosDTO());
        mockService.Setup(service => service.GetRestaurantesPendentes()).Returns(() => GetTestRestaurantesPendentesDTO());

        mockService.Setup(service => service.Get(It.Is<uint>(id => id == 1))).Returns(() => GetTargetRestaurantePendente());
        mockService.Setup(service => service.Get(It.Is<uint>(id => id == 2))).Returns(() => GetTargetRestauranteAprovado());
        mockService.Setup(service => service.Get(It.Is<uint>(id => id == 3))).Returns(() => GetTargetRestaurantePendente2());

        mockService.Setup(service => service.Edit(It.IsAny<Restaurante>())).Verifiable();
        mockService.Setup(service => service.Create(It.IsAny<Restaurante>())).Verifiable();
        mockService.Setup(service => service.Delete(It.IsAny<uint>())).Verifiable();

        controller = new RestauranteController(
            mockService.Object,
            mapper,
            mockUserManager.Object,
            mockRoleManager.Object,
            mockEmailSender.Object);

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

    private static RestauranteViewModel GetNovoRestauranteSolicitacao()
    {
        return new RestauranteViewModel
        {
            Id = 4,
            NomeRestaurante = "Novo Restaurante Solicitação",
            NomeProprietario = "Novo Proprietário",
            CpfProprietario = "123.456.789-00",
            Cnpj = "12.345.678/0001-90",
            Email = "novo@email.com",
            TelefoneProprietario = "(11) 99999-9999",
            TelefoneRestaurante = "(11) 8888-8888",
            Rua = "Rua Nova",
            Numero = "100",
            Bairro = "Bairro Novo",
            Cidade = "Cidade Nova",
            Estado = "SP",
            Cep = "12345678",
            Descricao = "Descrição do novo restaurante",
            Validado = false
        };
    }

    private static Restaurante GetTargetRestaurantePendente()
    {
        return new Restaurante
        {
            Id = 1,
            NomeRestaurante = "Restaurante Pendente 1",
            NomeProprietario = "Proprietário Pendente",
            CpfProprietario = "111.111.111-11",
            Cnpj = "11.111.111/0001-11",
            Email = "pendente1@email.com",
            TelefoneProprietario = "(11) 1111-1111",
            TelefoneRestaurante = "(11) 2222-2222",
            Rua = "Rua Pendente",
            Numero = "111",
            Bairro = "Bairro Pendente",
            Cidade = "Cidade Pendente",
            Estado = "SP",
            Cep = "11111111",
            Descricao = "Restaurante pendente de aprovação",
            Validado = false
        };
    }

    private static Restaurante GetTargetRestauranteAprovado()
    {
        return new Restaurante
        {
            Id = 2,
            NomeRestaurante = "Restaurante Aprovado 1",
            NomeProprietario = "Proprietário Aprovado",
            CpfProprietario = "222.222.222-22",
            Cnpj = "22.222.222/0001-22",
            Email = "aprovado@email.com",
            TelefoneProprietario = "(11) 3333-3333",
            TelefoneRestaurante = "(11) 4444-4444",
            Rua = "Rua Aprovada",
            Numero = "222",
            Bairro = "Bairro Aprovado",
            Cidade = "Cidade Aprovada",
            Estado = "RJ",
            Cep = "22222222",
            Descricao = "Restaurante já aprovado",
            Validado = true
        };
    }

    private static Restaurante GetTargetRestaurantePendente2()
    {
        return new Restaurante
        {
            Id = 3,
            NomeRestaurante = "Restaurante Pendente 2",
            NomeProprietario = "Proprietário Pendente 2",
            CpfProprietario = "333.333.333-33",
            Cnpj = "33.333.333/0001-33",
            Email = "pendente2@email.com",
            TelefoneProprietario = "(11) 5555-5555",
            TelefoneRestaurante = "(11) 6666-6666",
            Rua = "Rua Pendente 2",
            Numero = "333",
            Bairro = "Bairro Pendente 2",
            Cidade = "Cidade Pendente 2",
            Estado = "MG",
            Cep = "33333333",
            Descricao = "Segundo restaurante pendente",
            Validado = false
        };
    }

    private static List<RestauranteDTO> GetTestRestaurantesCompletosDTO()
    {
        return new List<RestauranteDTO>
        {
            new RestauranteDTO { Id = 1, NomeRestaurante = "Restaurante Pendente 1", Validado = false, Cidade = "Cidade Pendente", Estado = "SP" },
            new RestauranteDTO { Id = 2, NomeRestaurante = "Restaurante Aprovado 1", Validado = true, Cidade = "Cidade Aprovada", Estado = "RJ" },
            new RestauranteDTO { Id = 3, NomeRestaurante = "Restaurante Pendente 2", Validado = false, Cidade = "Cidade Pendente 2", Estado = "MG" }
        };
    }

    private static List<RestauranteDTO> GetTestRestaurantesPendentesDTO()
    {
        return new List<RestauranteDTO>
        {
            new RestauranteDTO { Id = 1, NomeRestaurante = "Restaurante Pendente 1", Validado = false, Cidade = "Cidade Pendente", Estado = "SP" },
            new RestauranteDTO { Id = 3, NomeRestaurante = "Restaurante Pendente 2", Validado = false, Cidade = "Cidade Pendente 2", Estado = "MG" }
        };
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando ListarSolicitacoes - deve retornar apenas pendentes")]
    public void ListarSolicitacoesTest()
    {
        var result = controller!.ListarSolicitacoes();

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        ViewResult viewResult = (ViewResult)result;

        Assert.IsInstanceOfType(viewResult.Model, typeof(List<RestauranteViewModel>));
        List<RestauranteViewModel> viewModel = (List<RestauranteViewModel>)viewResult.Model;
        Assert.AreEqual(2, viewModel.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando DetailsSolicitacao - visualização de solicitação pendente")]
    public void DetailsSolicitacaoTest()
    {
        var result = controller!.DetailsSolicitacao(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        ViewResult viewResult = (ViewResult)result;

        Assert.IsInstanceOfType(viewResult.Model, typeof(RestauranteViewModel));
        RestauranteViewModel viewModel = (RestauranteViewModel)viewResult.Model;

        Assert.AreEqual((uint)1, viewModel.Id);
        Assert.AreEqual("Restaurante Pendente 1", viewModel.NomeRestaurante);
        Assert.IsFalse(viewModel.Validado);
        Assert.AreEqual("Cidade Pendente", viewModel.Cidade);
        Assert.AreEqual("SP", viewModel.Estado);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando AprovarSolicitacao - deve alterar status para true")]
    public void AprovarSolicitacaoTest()
    {
        var restauranteParaAprovar = GetTargetRestaurantePendente();
        mockService!.Setup(s => s.Get(It.Is<uint>(id => id == 1))).Returns(() => restauranteParaAprovar);

        var result = Unwrap(controller!.AprovarSolicitacao(1));

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;

        Assert.AreEqual("ListarSolicitacoes", redirectToActionResult.ActionName);

        mockService.Verify(s => s.Edit(It.Is<Restaurante>(r =>
            r.Id == 1 && r.Validado == true)), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando NegarSolicitacao - deve excluir o restaurante")]
    public void NegarSolicitacaoTest()
    {
        uint idParaNegar = 3;

        var result = Unwrap(controller!.NegarSolicitacao(idParaNegar));

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;

        Assert.AreEqual("ListarSolicitacoes", redirectToActionResult.ActionName);

        mockService!.Verify(s => s.Delete(idParaNegar), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando SolicitarAdesao - GET")]
    public void SolicitarAdesaoTest_Get()
    {
        var result = controller!.SolicitarAdesao();

        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando SolicitarAdesao POST")]
    public void SolicitarAdesaoTest()
    {
        var novaSolicitacao = GetNovoRestauranteSolicitacao();

        var result = controller!.SolicitarAdesao(novaSolicitacao);

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;

        Assert.AreEqual("Index", redirectToActionResult.ActionName);

        mockService!.Verify(s => s.Create(It.IsAny<Restaurante>()), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando que Details não permite alterar status")]
    public void Details_NaoPermiteAlterarStatusTest()
    {
        var result = controller!.Details(1) as ViewResult;
        var viewModel = result?.Model as RestauranteViewModel;
        Assert.IsNotNull(viewModel);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando se DetailsSolicitacao permite alterar status")]
    public void DetailsSolicitacao_PermiteAlterarStatusTest()
    {
        var result = controller!.DetailsSolicitacao(1) as ViewResult;
        var viewModel = result?.Model as RestauranteViewModel;

        Assert.IsNotNull(viewModel);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando filtro de Index - apenas ativos")]
    public void Index_FiltraApenasAtivosTest()
    {
        var result = controller!.Index();

        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [Description("Testando Create - deve começar como pendente")]
    public void Create_DeveComecarComoPendenteTest()
    {
        var novoRestauranteViewModel = GetNovoRestauranteSolicitacao();

        var result = controller!.Create(novoRestauranteViewModel);

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

        mockService!.Verify(s => s.Create(It.Is<Restaurante>(r =>
            r.Validado == false)), Times.AtLeastOnce);
    }

    private static object? Unwrap(object? maybeTask)
    {
        if (maybeTask is null) return null;
        if (maybeTask is Task task)
        {
            task.GetAwaiter().GetResult();

            var taskType = task.GetType();
            if (taskType.IsGenericType)
            {
                var prop = taskType.GetProperty("Result");
                return prop?.GetValue(task);
            }

            return null;
        }

        return maybeTask;
    }
}