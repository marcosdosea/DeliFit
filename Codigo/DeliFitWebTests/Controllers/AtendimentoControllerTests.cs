using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Mappers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DeliFitWeb.Controllers.Tests;

[TestClass]
public class AtendimentoControllerTests
{
    private AtendimentoController controller = null!;
    private Mock<IAtendimentoService> mockService = null!;

    [TestInitialize]
    public void Initialize()
    {
        mockService = new Mock<IAtendimentoService>();

        IMapper mapper = new MapperConfiguration(cfg =>
            cfg.AddProfile(new AtendimentoProfile())).CreateMapper();

        mockService.Setup(s => s.GetAll(1))
            .Returns(GetTestAtendimento());

        mockService.Setup(s => s.Get(It.IsAny<uint>()))
            .Returns(GetTargetAtendimento());

        mockService.Setup(s => s.Create(It.IsAny<Atendimento>()));
        mockService.Setup(s => s.Edit(It.IsAny<Atendimento>()));
        mockService.Setup(s => s.Delete(It.IsAny<uint>()));

        controller = new AtendimentoController(mockService.Object, mapper);
    }

    [TestMethod]
    public void IndexTest_Valido()
    {
        var result = controller.Index(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var viewResult = (ViewResult)result;
        var lista = viewResult.Model as List<AtendimentoViewModel>;

        Assert.IsNotNull(lista);
        Assert.AreEqual(3, lista.Count);
    }

    [TestMethod]
    public void CreateTest_Get_Valido()
    {
        var result = controller.Create(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public void CreateTest_Post_Valid()
    {
        var result = controller.Create(GetNewAtendimento());

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
    }

    [TestMethod]
    public void CreateTest_Post_Invalid()
    {
        controller.ModelState.AddModelError("Dia da Semana", "Campo requerido");

        var result = controller.Create(GetNewAtendimento());

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
    }

    [TestMethod]
    public void EditTest_Get_Valid()
    {
        var result = controller.Edit(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public void EditTest_Post_Valid()
    {
        var model = GetTargetAtendimentoViewModel();
        var result = controller.Edit(model);

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
    }

    [TestMethod]
    public void DeleteTest_Get_Valid()
    {
        var result = controller.Delete(1);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public void DeleteTest_Post_Valid()
    {
        var model = GetTargetAtendimentoViewModel();
        var result = controller.Delete(model.Id, model);

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
    }

    private AtendimentoViewModel GetNewAtendimento()
    {
        return new AtendimentoViewModel
        {
            Id = 1,
            IdRestaurante = 1,
            DiaSemana = "Domingo",
            HorarioInicio = new DateTime(2024, 6, 10, 8, 0, 0),
            HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0)
        };
    }

    private AtendimentoViewModel GetTargetAtendimentoViewModel()
    {
        return new AtendimentoViewModel
        {
            Id = 1,
            IdRestaurante = 1,
            DiaSemana = "Domingo",
            HorarioInicio = new DateTime(2024, 6, 10, 8, 0, 0),
            HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0)
        };
    }

    private Atendimento GetTargetAtendimento()
    {
        return new Atendimento
        {
            Id = 1,
            IdRestaurante = 1,
            DiaSemana = "Domingo",
            HorarioInicio = new DateTime(2024, 6, 10, 8, 0, 0),
            HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0)
        };
    }

    private IEnumerable<Atendimento> GetTestAtendimento()
    {
        return new List<Atendimento>
        {
            new Atendimento
            {
                Id = 1,
                DiaSemana = "Domingo",
                HorarioInicio = new DateTime(2024, 6, 10, 8, 0, 0),
                HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0),
                IdRestaurante = 1
            },
            new Atendimento
            {
                Id = 2,
                DiaSemana = "Segunda-feira",
                HorarioInicio = new DateTime(2024, 6, 11, 8, 0, 0),
                HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0),
                IdRestaurante = 1
            },
            new Atendimento
            {
                Id = 3,
                DiaSemana = "Terça-feira",
                HorarioInicio = new DateTime(2024, 6, 12, 8, 0, 0),
                HorarioFim = new DateTime(2024, 6, 10, 18, 0, 0),
                IdRestaurante = 1
            }
        };
    }
}