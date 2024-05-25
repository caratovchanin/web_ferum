using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using webFerum.Models;
using webFerum.Models.AppContext;
using webFerum.Service;
using webFerum.Utils;




namespace webFerum.Controllers
{
    public class AuthController : Controller
    {
        private UserService userService;

        public AuthController(UserService userService) 
        {
            this.userService = userService; 
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IResult> Login([FromBody] AuthUser data)
        {
            if (data.isValid)
            {
                data.Password = Utils.Encrypt.sha256(data.Password);

                User? user = await userService.GetEmployeeAsync(new UserModel(){
                                                                    Email = data.Email, 
                                                                    Password = data.Password});

                if (user is null) Results.StatusCode(StatusCodes.Status400BadRequest);

                List<Claim> claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, data.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("RolePolicy", user.Role.Policy)
                };

                JwtSecurityToken jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                var response = new
                {
                    accessToken = new JwtSecurityTokenHandler().WriteToken(jwt)
                };

                HttpContext.Session.SetString("accessToken", response.accessToken);

                return Results.Json(response);
            }

            return Results.StatusCode(StatusCodes.Status400BadRequest);
        }
        
    }
}
