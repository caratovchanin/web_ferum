using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webFerum.Models.AppContext;
using webFerum.Service;

namespace webFerum.Controllers
{
    public class ClientController : Controller
    {
        private ClientService clientService;
        private UserService userService;
        public ClientController(ClientService clientService, UserService userService)
        {
            this.clientService = clientService;
            this.userService = userService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateClient([FromForm] Client client)
        {
            clientService.AddClientAsync(client);
            TgService.service.AnswerToClientAsync(client);
            return Ok();
        }
    }
}
