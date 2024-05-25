using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webFerum.Models.AppContext;
using webFerum.Service;

namespace webFerum.Controllers
{
    public class ItemController : Controller
    {
        private ItemService itemService;
        private UserService userService;
        public ItemController(ItemService itemService, UserService userService)
        {
            this.itemService = itemService;
            this.userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Selected(int id)
        {
            Item item = await itemService.GetItemAsync(id);
            return View(item);
        }

        [HttpGet]
        [Authorize(Policy = "Operator")]
        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "Operator")]
        public async Task<IActionResult> Edit(int id)
        {
            Item item = await itemService.GetItemAsync(id);
            return View(item);
        }

        [HttpPost]
        [Authorize(Policy = "Operator")]
        public async Task<IActionResult> Create([FromForm] Item data)
        {
            int id = await itemService.AddItemAsync(data);

            return RedirectToAction("Selected", "Item", new { id = id });
        }

        [HttpPost]
        [Authorize(Policy = "Operator")]
        public async Task<IActionResult> Update([FromForm] Item data)
        {
            int id = await itemService.UpdateItemAsync(data);

            return RedirectToAction("Selected", "Item", new { id = id } );
        }
    }
}
