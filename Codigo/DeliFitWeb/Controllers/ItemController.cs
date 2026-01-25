using AutoMapper;
using Core;
using Core.Service;
using DeliFitWeb.Models;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult Index()
        {
            var listaItens = _itemService.GetAll();
            var listaItensModel = _mapper.Map<List<ItemModel>>(listaItens);
            return View(listaItensModel);//TO-DO
        }

        // GET: ItemController/Details/5
        public ActionResult Details(uint id)
        {
            Item? item = _itemService.Get(id);
            ItemModel itemModel = _mapper.Map<ItemModel>(item);
            return View(itemModel);//TO-DO
        }

        // GET: ItemController/Create
        public ActionResult Create()
        {
            return View();//TO-DO
        }

        // POST: ItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemModel itemModel)
        {
            if (ModelState.IsValid)
            {
                var item = _mapper.Map<Item>(itemModel);
                _itemService.Create(item);
            }
            return RedirectToAction(nameof(Index));//TO-DO
        }

        // GET: ItemController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();//TO-DO
        }

        // POST: ItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ItemModel itemModel)
        {
            if (ModelState.IsValid)
            {
                var item = _mapper.Map<Item>(itemModel);
                _itemService.Edit(item);
            }
            return RedirectToAction(nameof(Index));//TO-DO
        }

        // GET: ItemController/Delete/5
        public ActionResult Delete(uint id)
        {
            Item item = _itemService.Get(id);
            ItemModel itemModel = _mapper.Map<ItemModel>(item);
            return View(itemModel);//TO-DO
        }

        // POST: ItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(uint id, ItemModel itemModel)
        {
            _itemService.Delete(id);
            return RedirectToAction(nameof(Index));//TO-DO
        }
    }
}
