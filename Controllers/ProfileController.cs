using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using webFerum.Service;
using webFerum.Models.AppContext;

namespace webFerum.Controllers
{
    
    public class ProfileController : Controller
    {
        private RoleService roleService;
        private UserService userService;
        public ProfileController(UserService userService, RoleService roleService) 
        {
            this.roleService = roleService;
            this.userService = userService;
        }

        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Claim> claims = User.Claims;
            Claim claim = claims.Where(claim => claim.Type == ClaimTypes.NameIdentifier).First();
            string id = claim.Value;

            User user = await userService.GetEmployeeAsync(int.Parse(id));

            return View(user);
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AProfile(int Id)
        {
            User user = await userService.GetEmployeeAsync(Id);

            return View(user);
        }

        [HttpGet()]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> All()
        {
            return View();
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id) 
        {
            User user = await userService.GetEmployeeAsync(id);

            return View(user);
        }

        [HttpPost()]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Update([FromForm] User data)
        {
            int id = await userService.UpdateUserAsync(data);

            return RedirectToAction("AProfile", "Profile", new { id = id });
        }

    }
}

