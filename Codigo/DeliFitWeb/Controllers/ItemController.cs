using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using DeliFitWeb.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace DeliFitWeb.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;
        private readonly IRestauranteService _restauranteService;
        private readonly ICategoriaService _categoriaService;

        public ItemController(IItemService itemService, IMapper mapper, IRestauranteService restauranteService, ICategoriaService categoriaService)
        {
            _itemService = itemService;
            _mapper = mapper;
            _restauranteService = restauranteService;
            _categoriaService = categoriaService;
        }

        // Método auxiliar para popular a lista de categorias disponíveis nas views de Create/Edit
        private void CarregarCategoriasDisponiveis()
        {
            ViewBag.CategoriasDisponiveis = _categoriaService.ListarCategorias();
        }

        // GET: ItemController
        [Authorize(Roles = "GerenteRestaurante,Admin")]
        public ActionResult Index(uint? idRestaurante)
        {
            IEnumerable<Item> listaItens;

            // Se for passado um ID na URL, usa ele (Admin pode ver de qualquer restaurante)
            if (idRestaurante.HasValue)
            {
                listaItens = _itemService.GetByRestaurante(idRestaurante.Value);
            }
            else if (User.IsInRole("GerenteRestaurante"))
            {
                // Se for gerente, mostra apenas os itens do seu restaurante
                var restauranteId = GetRestauranteIdLogado();
                if (restauranteId.HasValue)
                {
                    listaItens = _itemService.GetByRestaurante(restauranteId.Value);
                }
                else
                {
                    TempData["Error"] = "Não foi possível identificar o restaurante. Faça login novamente.";
                    return RedirectToAction("Home", "Restaurante");
                }
            }
            else
            {
                // Admin sem filtro - mostra todos
                listaItens = _itemService.GetAll();
            }

            var listaItensModel = _mapper.Map<List<ItemViewModel>>(listaItens);
            return View(listaItensModel);
        }

        // GET: ItemController/Details/5
        public ActionResult Details(uint id)
        {
            Item? item = _itemService.Get(id);
            ItemViewModel itemModel = _mapper.Map<ItemViewModel>(item);
            return View(itemModel);
        }

        /// <summary>
        /// Serve a foto de um item diretamente do banco para uso nas views via <img src="...">.
        /// </summary>
        [HttpGet]
        public ActionResult Foto(uint id)
        {
            var item = _itemService.Get(id);

            if (item?.Foto == null || item.Foto.Length == 0)
                return NotFound();

            return File(item.Foto, "image/jpeg");
        }

        // GET: ItemController/Create
        [Authorize(Roles = "GerenteRestaurante")]
        public ActionResult Create(uint? idRestaurante)
        {
            var model = new ItemViewModel();

            // Se foi passado um ID na URL, usa ele
            if (idRestaurante.HasValue)
            {
                model.IdRestaurante = idRestaurante.Value;
            }
            else
            {
                // Senão, tenta buscar da sessão (gerente de restaurante logado)
                var restauranteId = GetRestauranteIdLogado();
                if (restauranteId.HasValue)
                {
                    model.IdRestaurante = restauranteId.Value;
                }
                else
                {
                    TempData["Error"] = "Não foi possível identificar o restaurante. Faça login novamente.";
                    return RedirectToAction("Home", "Restaurante");
                }
            }

            CarregarCategoriasDisponiveis();
            return View(model);
        }

        // POST: ItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GerenteRestaurante")]
        public ActionResult Create(ItemViewModel itemModel)
        {
            // Se o IdRestaurante não foi enviado, tenta buscar da sessão
            if (itemModel.IdRestaurante == 0)
            {
                var restauranteId = GetRestauranteIdLogado();
                if (restauranteId.HasValue)
                {
                    itemModel.IdRestaurante = restauranteId.Value;
                }
                else
                {
                    ModelState.AddModelError("", "Não foi possível identificar o restaurante. Faça login novamente.");
                    CarregarCategoriasDisponiveis();
                    return View(itemModel);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    itemModel.Restricao = CombinarRestricoes(itemModel.RestricoesSelecionadas);
                    var item = _mapper.Map<Item>(itemModel);

                    // Trata a foto manualmente após o mapeamento
                    if (itemModel.FotoFile != null && itemModel.FotoFile.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        itemModel.FotoFile.CopyTo(ms);
                        item.Foto = ms.ToArray();
                    }

                    var itemId = _itemService.Create(item, itemModel.CategoriaIds);
                    TempData["Success"] = $"Item '{item.Nome}' criado com sucesso!";
                    return RedirectToAction(nameof(Index), new { idRestaurante = itemModel.IdRestaurante });
                }
                catch (ServiceException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao criar item: {ex.Message}");
                }
            }

            CarregarCategoriasDisponiveis();
            return View(itemModel);
        }

        private static string? CombinarRestricoes(List<string> restricoesSelecionadas)
        {
            return restricoesSelecionadas != null && restricoesSelecionadas.Any()
                ? string.Join(", ", restricoesSelecionadas)
                : null;
        }

        private static List<string> SepararRestricoes(string? restricao)
        {
            return string.IsNullOrWhiteSpace(restricao)
                ? new List<string>()
                : restricao.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        // GET: ItemController/Edit/5
        public ActionResult Edit(uint id)
        {
            Item? item = _itemService.Get(id);
            if (item == null)
                return NotFound();

            ItemViewModel itemModel = _mapper.Map<ItemViewModel>(item);
            itemModel.RestricoesSelecionadas = SepararRestricoes(itemModel.Restricao);
            CarregarCategoriasDisponiveis();
            return View(itemModel);
        }

        // POST: ItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ItemViewModel itemModel)
        {
            if (ModelState.IsValid)
            {
                itemModel.Restricao = CombinarRestricoes(itemModel.RestricoesSelecionadas);
                var item = _mapper.Map<Item>(itemModel);

                // Trata a foto manualmente após o mapeamento
                if (itemModel.FotoFile != null && itemModel.FotoFile.Length > 0)
                {
                    // Nova foto enviada: converte e salva
                    using var ms = new MemoryStream();
                    itemModel.FotoFile.CopyTo(ms);
                    item.Foto = ms.ToArray();
                }
                else
                {
                    // Nenhuma foto nova: preserva a foto já existente no banco
                    var itemExistente = _itemService.Get(itemModel.Id);
                    item.Foto = itemExistente?.Foto;
                }

                _itemService.Edit(item, itemModel.CategoriaIds);
                return RedirectToAction(nameof(Index), new { idRestaurante = itemModel.IdRestaurante });
            }

            CarregarCategoriasDisponiveis();
            return View(itemModel);
        }

        // GET: ItemController/Delete/5
        public ActionResult Delete(uint id)
        {
            Item? item = _itemService.Get(id);
            if (item == null)
                return NotFound();

            ItemViewModel itemModel = _mapper.Map<ItemViewModel>(item);
            return View(itemModel);
        }

        // POST: ItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, ItemViewModel itemModel)
        {
            _itemService.Delete(id);
            return RedirectToAction(nameof(Index), new { idRestaurante = itemModel.IdRestaurante });
        }

        // Método auxiliar para obter o ID do restaurante logado
        private uint? GetRestauranteIdLogado()
        {
            // Tenta buscar da sessão
            var restauranteId = HttpContext.Session.GetRestauranteId();

            if (!restauranteId.HasValue)
            {
                // Se não estiver na sessão, busca pelo email
                var userEmail = User.Identity?.Name;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var restaurante = _restauranteService.GetByEmail(userEmail);

                    if (restaurante != null)
                    {
                        // Armazena na sessão para próximas requisições
                        HttpContext.Session.SetRestauranteId(restaurante.Id);
                        restauranteId = restaurante.Id;
                    }
                }
            }

            return restauranteId;
        }
    }
}