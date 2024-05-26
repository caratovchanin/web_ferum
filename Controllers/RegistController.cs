using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webFerum.Models;
using webFerum.Models.AppContext;
using webFerum.Service;
using webFerum.Utils;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Web;
using System.Security.Claims;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace webFerum.Controllers
{
    public class RegistController : Controller
    {
        private UserService userService;

        public RegistController(UserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IResult> Add([FromBody] UserModel data)
        {
            data.Password = Encrypt.sha256(data.Password);
            int id = await userService.AddUserAsync(data);

            return Results.Json(new { Id = id });
        }

        [HttpPost]
        public async Task<IResult> Reg([FromBody] UserModel data)
        {
            data.Password = Encrypt.sha256(data.Password);
            await userService.AddUserAsync(data);

            return Results.Ok();
        }
    }
}
