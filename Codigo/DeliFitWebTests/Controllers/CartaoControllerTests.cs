using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Controllers;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using DeliFitWeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace DeliFitWebTests.Controllers;

[TestClass]
public class CartaoControllerTests
{
    private CartaoController controller = null!;
    private Mock<ICartaoService> mockService = null!;
    private Mock<IMercadoPagoService> mockMercadoPagoService = null!;

    [TestInitialize]
    public void Initialize()
    {
        mockService = new Mock<ICartaoService>();
        var mockClienteService = new Mock<IClienteService>();
        mockMercadoPagoService = new Mock<IMercadoPagoService>();
        var mockConfiguration = new Mock<IConfiguration>();

        IMapper mapper = new MapperConfiguration(cfg =>
            cfg.AddProfile(new CartaoProfile())).CreateMapper();

        mockService.Setup(s => s.GetByCliente(It.IsAny<uint>()))
            .Returns(GetTestCartaos());

        mockService.Setup(s => s.Get(It.IsAny<uint>()))
            .Returns(GetTargetCartao());

        mockService.Setup(s => s.Create(It.IsAny<Cartao>()));
        mockService.Setup(s => s.Delete(It.IsAny<uint>()));

        mockClienteService.Setup(s => s.Get(It.IsAny<uint>()))
            .Returns(new Cliente { Id = 1, Nome = "Kauan Brilhante", Email = "teste@email.com", Cpf = "11111111111", Telefone = "11999999999" });

        mockMercadoPagoService.Setup(s => s.ObterOuCriarCustomerIdAsync(It.IsAny<Cliente>()))
            .ReturnsAsync("customer-1");
        mockMercadoPagoService.Setup(s => s.SalvarCartaoAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new CartaoSalvoResultado
            {
                Sucesso = true,
                MercadoPagoCardId = "card-4",
                PaymentMethodId = "master",
                Bandeira = "Mastercard",
                UltimosQuatroDigitos = "1234",
                ExpirationMonth = 5,
                ExpirationYear = 2030
            });

        controller = new CartaoController(
            mockService.Object,
            mapper,
            mockClienteService.Object,
            mockMercadoPagoService.Object,
            mockConfiguration.Object);

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

    [TestMethod]
    public void IndexTest_Valido()
    {
        var result = controller.Index(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var viewResult = (ViewResult)result;
        var lista = viewResult.Model as List<CartaoViewModel>;

        Assert.IsNotNull(lista);
        Assert.AreEqual(3, lista.Count);
    }

    [TestMethod]
    public void DetailsTest_Valido()
    {
        var result = controller.Details(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var model = ((ViewResult)result).Model as CartaoViewModel;

        Assert.IsNotNull(model);
        Assert.AreEqual("Kauan Brilhante", model.Nome);
    }

    [TestMethod]
    public void CreateTest_Get_Valido()
    {
        var result = controller.Create(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public async Task CreateTest_Post_Valid()
    {
        var result = await controller.Create(GetNewCartao());

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        mockMercadoPagoService.Verify(s => s.ObterOuCriarCustomerIdAsync(It.IsAny<Cliente>()), Times.Once);
        mockMercadoPagoService.Verify(s => s.SalvarCartaoAsync("customer-1", "token-teste"), Times.Once);
    }

    [TestMethod]
    public async Task CreateTest_Post_Invalid()
    {
        controller.ModelState.AddModelError("Nome", "Campo requerido");

        var result = await controller.Create(GetNewCartao());

        // Formulário inválido reexibe a própria tela de cadastro com os erros
        // ao lado dos campos, em vez de redirecionar para a lista sem explicação.
        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public async Task CreateTest_Post_SemToken_Invalido()
    {
        var model = GetNewCartao();
        model.Token = null;

        var result = await controller.Create(model);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public void DeleteTest_Get_Valid()
    {
        var result = controller.Delete(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public async Task DeleteTest_Post_Valid()
    {
        var model = GetTargetCartaoViewModel();
        var result = await controller.Delete(model.Id, model);

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
    }

    private CartaoViewModel GetNewCartao()
    {
        return new CartaoViewModel
        {
            Id = 4,
            IdCliente = 1,
            Nome = "Novo Cartao",
            Cpf = "12312312312",
            Token = "token-teste"
        };
    }

    private CartaoViewModel GetTargetCartaoViewModel()
    {
        return new CartaoViewModel
        {
            Id = 1,
            IdCliente = 1,
            Nome = "Kauan Brilhante",
            Cpf = "11111111111",
            Bandeira = "Mastercard",
            UltimosQuatroDigitos = "4444",
            Validade = new DateTime(2027, 12, 1)
        };
    }

    private Cartao GetTargetCartao()
    {
        return new Cartao
        {
            Id = 1,
            IdCliente = 1,
            Nome = "Kauan Brilhante",
            Cpf = "11111111111",
            MercadoPagoCardId = "card-1",
            MercadoPagoPaymentMethodId = "master",
            Bandeira = "Mastercard",
            UltimosQuatroDigitos = "4444",
            Validade = new DateTime(2027, 12, 1)
        };
    }

    private IEnumerable<Cartao> GetTestCartaos()
    {
        return new List<Cartao>
        {
            new Cartao
            {
                Id = 1,
                IdCliente = 1,
                Nome = "Kauan Brilhante",
                Cpf = "11111111111",
                MercadoPagoCardId = "card-1",
                MercadoPagoPaymentMethodId = "master",
                Bandeira = "Mastercard",
                UltimosQuatroDigitos = "4444",
                Validade = new DateTime(2027, 12, 1)
            },
            new Cartao
            {
                Id = 2,
                IdCliente = 1,
                Nome = "Kauan Brilhante",
                Cpf = "22222222222",
                MercadoPagoCardId = "card-2",
                MercadoPagoPaymentMethodId = "visa",
                Bandeira = "Visa",
                UltimosQuatroDigitos = "8888",
                Validade = new DateTime(2028, 6, 1)
            },
            new Cartao
            {
                Id = 3,
                IdCliente = 1,
                Nome = "Outro Nome",
                Cpf = "33333333333",
                MercadoPagoCardId = "card-3",
                MercadoPagoPaymentMethodId = "visa",
                Bandeira = "Visa",
                UltimosQuatroDigitos = "2222",
                Validade = new DateTime(2029, 1, 1)
            }
        };
    }
}
