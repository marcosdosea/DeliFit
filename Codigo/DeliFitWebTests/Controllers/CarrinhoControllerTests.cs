using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DeliFitWeb.Controllers.Tests;

[TestClass()]
public class CarrinhoControllerTests
{
    private static CarrinhoController? controller;

    [TestInitialize]
    public void Initialize()
    {
        var mockService = new Mock<ICarrinhoService>();
        var mockClienteService = new Mock<IClienteService>();
        var mockCartaoService = new Mock<ICartaoService>();

        IMapper mapper = new MapperConfiguration(cfg =>
            cfg.AddProfile(new CarrinhoProfile())).CreateMapper();

        mockService.Setup(service => service.GetAll())
            .Returns(GetTestCarrinho());
        mockService.Setup(service => service.Get(1))
            .Returns(GetTargetCarrinho());
        mockService.Setup(service => service.Create(It.IsAny<Carrinho>()))
            .Verifiable();
        mockService.Setup(service => service.Delete(It.IsAny<uint>()));

        controller = new CarrinhoController(mockService.Object, mockClienteService.Object, mockCartaoService.Object, mapper);
    }

    [TestMethod()]
    [TestCategory("Unit")]
    [Description("Testando o index")]
    public void IndexTest_Valido()
    {
        var result = controller?.Index();

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        ViewResult viewResult = (ViewResult)result;
        Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<CarrinhoViewModel>));

        List<CarrinhoViewModel>? lista = (List<CarrinhoViewModel>)viewResult.ViewData.Model;
        Assert.AreEqual(3, lista.Count);
    }

    [TestMethod()]
    public void DetailsTest_Valido()
    {
        var result = controller?.Details(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        ViewResult viewResult = (ViewResult)result;
        Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CarrinhoViewModel));
        CarrinhoViewModel carrinhoModel = (CarrinhoViewModel)viewResult.ViewData.Model;
        Assert.AreEqual("Carne ao ponto", carrinhoModel.Observacao);
        Assert.AreEqual("D", carrinhoModel.FormaDePagamento);
    }

    [TestMethod()]
    public void CreateTest_Get_Valido()
    {
        var result = controller?.Create();
        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod()]
    public void CreateTest_Valid()
    {
        var result = controller?.Create(GetNewCarrinho());

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
        Assert.IsNull(redirectToActionResult.ControllerName);
        Assert.AreEqual("Index", redirectToActionResult.ActionName);
    }

    [TestMethod()]
    public void CreateTest_Post_Invalid()
    {
        controller?.ModelState.AddModelError("FormaDePagamento", "Forma de Pagamento é obrigatório.");

        var result = controller?.Create(GetNewCarrinho());

        Assert.AreEqual(1, controller?.ModelState.ErrorCount);
        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
        Assert.IsNull(redirectToActionResult.ControllerName);
        Assert.AreEqual("Index", redirectToActionResult.ActionName);
    }

    [TestMethod()]
    public void DeleteTest_Get_Valid()
    {
        var result = controller?.Delete(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        ViewResult viewResult = (ViewResult)result;
        Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(CarrinhoViewModel));
        CarrinhoViewModel carrinhoModel = (CarrinhoViewModel)viewResult.ViewData.Model;
        Assert.AreEqual("Carne ao ponto", carrinhoModel.Observacao);
        Assert.AreEqual("D", carrinhoModel.FormaDePagamento);
    }

    [TestMethod()]
    public void DeleteTest_Post_Valid()
    {
        var result = controller?.Delete(GetTargetCarrinhoModel());

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
        Assert.IsNull(redirectToActionResult.ControllerName);
        Assert.AreEqual("Index", redirectToActionResult.ActionName);
    }

    private CarrinhoViewModel GetNewCarrinho()
    {
        return new CarrinhoViewModel
        {
            Id = 4,
            Observacao = "Sem molho",
            IdCliente = 1,
            FormaDePagamento = "P",
            ValorFrete = 5.50M,
            IdCartao = null
        };
    }

    private static Carrinho GetTargetCarrinho()
    {
        return new Carrinho
        {
            Id = 1,
            Observacao = "Carne ao ponto",
            IdCliente = 2,
            FormaDePagamento = "D",
            ValorFrete = 10.50M,
            IdCartao = null
        };
    }

    private CarrinhoViewModel GetTargetCarrinhoModel()
    {
        return new CarrinhoViewModel
        {
            Id = 2,
            Observacao = "Sem cebola",
            IdCliente = 2,
            FormaDePagamento = "C",
            ValorFrete = 10.50M,
            IdCartao = 1
        };
    }

    private IEnumerable<Carrinho> GetTestCarrinho()
    {
        return new List<Carrinho>
        {
            new Carrinho
            {
                Id = 1,
                Observacao = "Carne ao ponto",
                IdCliente = 2,
                FormaDePagamento = "D",
                ValorFrete = 10.50M,
                IdCartao = null
            },
            new Carrinho
            {
                Id = 2,
                Observacao = "Sem cebola",
                IdCliente = 2,
                FormaDePagamento = "C",
                ValorFrete = 10.50M,
                IdCartao = 1
            },
            new Carrinho
            {
                Id = 3,
                Observacao = "Vegano",
                IdCliente = 3,
                FormaDePagamento = "P",
                ValorFrete = 8.00M,
                IdCartao = null
            }
        };
    }
}