using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Helpers;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitWeb.Controllers;

public class AtendimentoController : Controller
{
    private readonly IAtendimentoService _atendimentoService;
    private readonly IRestauranteService _restauranteService;
    private readonly IMapper _mapper;

    public AtendimentoController(IAtendimentoService atendimentoService, IRestauranteService restauranteService, IMapper mapper)
    {
        _atendimentoService = atendimentoService;
        _restauranteService = restauranteService;
        _mapper = mapper;
    }

    // Mesmo padrão usado em RestauranteController/ItemController/PagamentoController:
    // tenta a sessão primeiro e, se ausente, resolve pelo email do usuário logado.
    private uint? GetRestauranteIdLogado()
    {
        var restauranteId = HttpContext.Session.GetRestauranteId();

        if (!restauranteId.HasValue)
        {
            var userEmail = User.Identity?.Name;
            if (!string.IsNullOrEmpty(userEmail))
            {
                var restaurante = _restauranteService.GetByEmail(userEmail);
                if (restaurante != null)
                {
                    HttpContext.Session.SetRestauranteId(restaurante.Id);
                    restauranteId = restaurante.Id;
                }
            }
        }

        return restauranteId;
    }

    // GET: AtendimentoController
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public ActionResult Index(uint? idRestaurante = null)
    {
        var restauranteId = idRestaurante ?? GetRestauranteIdLogado();

        if (!restauranteId.HasValue || restauranteId.Value == 0)
        {
            TempData["Error"] = "Não foi possível identificar o restaurante.";
            return RedirectToAction("Home", "Restaurante");
        }

        var listaAtendimentos = _atendimentoService.GetAll(restauranteId.Value);
        var listaAtendimentosViewModel = _mapper.Map<IEnumerable<Core.Atendimento>, IEnumerable<DeliFitWeb.Models.AtendimentoViewModel>>(listaAtendimentos);

        ViewBag.IdRestaurante = restauranteId.Value;

        return View(listaAtendimentosViewModel);
    }

    // GET: AtendimentoController/Create
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public ActionResult Create(uint? idRestaurante)
    {
        var restauranteId = idRestaurante ?? GetRestauranteIdLogado();

        if (!restauranteId.HasValue || restauranteId.Value == 0)
        {
            TempData["Error"] = "Não foi possível identificar o restaurante.";
            return RedirectToAction("Home", "Restaurante");
        }

        var model = new AtendimentoViewModel { IdRestaurante = restauranteId.Value };
        return View(model);
    }

    // POST: AtendimentoController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public ActionResult Create(AtendimentoViewModel atendimentoModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var atendimento = _mapper.Map<Atendimento>(atendimentoModel);
                _atendimentoService.Create(atendimento);
                return RedirectToAction(nameof(Index), new { idRestaurante = atendimentoModel.IdRestaurante });
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        return View(atendimentoModel);
    }

    // GET: AtendimentoController/Edit/5
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public ActionResult Edit(uint id)
    {
        Atendimento? atendimento = _atendimentoService.Get(id);
        if (atendimento == null)
            return NotFound();

        return View(_mapper.Map<AtendimentoViewModel>(atendimento));
    }

    // POST: AtendimentoController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public ActionResult Edit(AtendimentoViewModel atendimentoModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var atendimento = _mapper.Map<Atendimento>(atendimentoModel);
                _atendimentoService.Edit(atendimento);
                return RedirectToAction(nameof(Index), new { idRestaurante = atendimentoModel.IdRestaurante });
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        return View(atendimentoModel);
    }

    // GET: AtendimentoController/Configurar
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public ActionResult Configurar(uint? idRestaurante = null)
    {
        var restauranteId = idRestaurante ?? GetRestauranteIdLogado();

        if (!restauranteId.HasValue || restauranteId.Value == 0)
        {
            TempData["Error"] = "Não foi possível identificar o restaurante.";
            return RedirectToAction("Home", "Restaurante");
        }

        var existentes = _atendimentoService.GetAll(restauranteId.Value)
            .GroupBy(a => a.DiaSemana)
            .ToDictionary(g => g.Key, g => g.First());

        var diasSemana = new[]
        {
            ("1", "Domingo"),
            ("2", "Segunda-feira"),
            ("3", "Terça-feira"),
            ("4", "Quarta-feira"),
            ("5", "Quinta-feira"),
            ("6", "Sexta-feira"),
            ("7", "Sábado"),
        };

        var model = new ConfigurarAtendimentoViewModel
        {
            IdRestaurante = restauranteId.Value,
            Dias = diasSemana.Select(d =>
            {
                existentes.TryGetValue(d.Item1, out var existing);
                return new DiaAtendimentoViewModel
                {
                    Id = existing?.Id ?? 0,
                    DiaSemana = d.Item1,
                    NomeDia = d.Item2,
                    Ativo = existing != null,
                    HorarioInicio = existing?.HorarioInicio?.ToString("HH:mm") ?? "08:00",
                    HorarioFim = existing?.HorarioFim?.ToString("HH:mm") ?? "18:00",
                };
            }).ToList()
        };

        return View(model);
    }

    // POST: AtendimentoController/Configurar
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public ActionResult Configurar(ConfigurarAtendimentoViewModel model)
    {
        if (model.IdRestaurante == 0)
        {
            TempData["Error"] = "Não foi possível identificar o restaurante.";
            return RedirectToAction("Home", "Restaurante");
        }

        var erros = new List<string>();

        foreach (var dia in model.Dias)
        {
            if (!TimeSpan.TryParse(dia.HorarioInicio, out var tsInicio))
                tsInicio = TimeSpan.FromHours(8);
            if (!TimeSpan.TryParse(dia.HorarioFim, out var tsFim))
                tsFim = TimeSpan.FromHours(18);

            var horarioInicio = DateTime.Today.Add(tsInicio);
            var horarioFim = DateTime.Today.Add(tsFim);

            if (dia.Ativo)
            {
                try
                {
                    if (dia.Id == 0)
                    {
                        _atendimentoService.Create(new Atendimento
                        {
                            DiaSemana = dia.DiaSemana,
                            HorarioInicio = horarioInicio,
                            HorarioFim = horarioFim,
                            IdRestaurante = model.IdRestaurante
                        });
                    }
                    else
                    {
                        _atendimentoService.Edit(new Atendimento
                        {
                            Id = dia.Id,
                            DiaSemana = dia.DiaSemana,
                            HorarioInicio = horarioInicio,
                            HorarioFim = horarioFim,
                            IdRestaurante = model.IdRestaurante
                        });
                    }
                }
                catch (ServiceException ex)
                {
                    erros.Add($"{dia.NomeDia}: {ex.Message}");
                }
            }
            else if (dia.Id > 0)
            {
                _atendimentoService.Delete(dia.Id);
            }
        }

        if (erros.Count > 0)
            TempData["Error"] = string.Join(Environment.NewLine, erros);
        else
            TempData["Success"] = "Horários configurados com sucesso!";

        return RedirectToAction(nameof(Configurar), new { idRestaurante = model.IdRestaurante });
    }

    // GET: AtendimentoController/Delete/5
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public ActionResult Delete(uint id)
    {
        Atendimento? atendimento = _atendimentoService.Get(id);
        if (atendimento == null)
            return NotFound();

        return View(_mapper.Map<AtendimentoViewModel>(atendimento));
    }

    // POST: AtendimentoController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "GerenteRestaurante,Admin")]
    public ActionResult Delete(uint id, AtendimentoViewModel atendimentoModel)
    {
        try
        {
            _atendimentoService.Delete(id);
        }
        catch (ServiceException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { idRestaurante = atendimentoModel.IdRestaurante });
    }
}
