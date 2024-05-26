using Microsoft.AspNetCore.Mvc;
using webFerum.Models.AppContext;
using webFerum.Service;
using webFerum.Models;
using Microsoft.AspNetCore.Authorization;

namespace webFerum.Controllers
{
    public class ArticleController : Controller
    {
        private ArticleService articleService;
        private UserService userService;
        public ArticleController(ArticleService articleService, UserService userService)
        {
            this.articleService = articleService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Selected(int id)
        {
            Article art = await articleService.GetAtricleAsync(id);
            return View(art);
        }

        [HttpGet]
        [Authorize(Policy = "Editor")]
        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "Editor")]
        public async Task<IActionResult> Edit(int id)
        {
            Article art = await articleService.GetAtricleAsync(id);
            return View(art);
        }

        [HttpPost]
        [Authorize(Policy = "Editor")]
        public async Task<IResult> Create([FromBody]Article data)
        {
            int id = await articleService.AddArticleAsync(data);

            return Results.Json( new { id = id });
        }

        [HttpPost]
        [Authorize(Policy = "Editor")]
        public async Task<IActionResult> Update([FromForm] Article data)
        {
            int id = await articleService.UpdateArticleAsync(data);

            return RedirectToAction("Selected", "Article", new { id = id });
        }

    }
}
