using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using webFerum.Models.AppContext;

namespace webFerum.Service
{
    public class ClientService
    {
        private IDistributedCache cache;
        private WebFerumContext context;
        public ClientService(IDistributedCache cache, WebFerumContext context)
        {
            this.cache = cache;
            this.context = context;
        }

        public async Task<int> AddClientAsync(Client client)
        {
            try
            {
                context.Clients.Add(client);

                await context.SaveChangesAsync();
                await context.Clients.LoadAsync();

                int id = context.Clients.Where(cl => cl.IdUser == client.IdUser && cl.Message == client.Message).First().Id;

                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"add client err: {ex.Message}");
            }
            return 0;
        }
    }
}
