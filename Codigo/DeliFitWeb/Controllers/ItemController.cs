using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace DeliFitWeb.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;

        public ItemController(IItemService itemService, IMapper mapper)
        {
            _itemService = itemService;
            _mapper = mapper;
        }
        // GET: ItemController
        public ActionResult Index(int v)
        {
            // 1. Buscamos no serviço filtrando pelo ID do restaurante
            // Certifique-se de que seu serviço possua um método que aceite o filtro
            var listaItens = _itemService.GetAll().Where(i => i.IdRestaurante == idRestaurante).ToList();

            // 2. Mapeamos apenas os itens filtrados para a ViewModel
            var listaItensModel = _mapper.Map<List<ItemViewModel>>(listaItens);

            // 3. (Opcional) Passamos o ID para a View via ViewBag para facilitar o uso nos links
            ViewBag.IdRestaurante = idRestaurante;

            return View(listaItensModel);
        }

        // GET: ItemController/Details/5
        public ActionResult Details(uint id)
        {
            Item? item = _itemService.Get(id);
            ItemViewModel itemModel = _mapper.Map<ItemViewModel>(item);
            return View(itemModel);
        }

        // GET: ItemController/Create
        public ActionResult Create(uint? idRestaurante)
        {
            var model = new ItemViewModel();
            if (idRestaurante.HasValue)
            {
                model.IdRestaurante = idRestaurante.Value;
            }
            return View(model);
        }

        // POST: ItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemViewModel itemModel)
        {
            if (ModelState.IsValid)
            {
                var item = _mapper.Map<Item>(itemModel);
                _itemService.Create(item);
                return RedirectToAction(nameof(Index));
            }
            return View(itemModel);
        }

        // GET: ItemController/Edit/5
        public ActionResult Edit(uint id)
        {
            Item? item = _itemService.Get(id);
            ItemViewModel itemModel = _mapper.Map<ItemViewModel>(item);

            return View(itemModel);
        }

        // POST: ItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ItemViewModel itemModel)
        {
            if (ModelState.IsValid)
            {
                var item = _mapper.Map<Item>(itemModel);
                _itemService.Edit(item);
                return RedirectToAction(nameof(Index));
            }
            return View(itemModel);
        }

        // GET: ItemController/Delete/5
        public ActionResult Delete(uint id)
        {
            Item item = _itemService.Get(id);
            ItemViewModel itemModel = _mapper.Map<ItemViewModel>(item);
            return View(itemModel);
        }

        // POST: ItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, ItemViewModel itemModel)
        {
            _itemService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
