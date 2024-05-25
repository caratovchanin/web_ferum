using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using webFerum.Models;
using webFerum.Models.AppContext;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Net.Http.Headers;

namespace webFerum.Service
{
    public class RoleService
    {
        private IDistributedCache cache;
        private WebFerumContext context;

        public RoleService(WebFerumContext ferrumContext, IDistributedCache distributedCache)
        {
            cache = distributedCache;
            context = ferrumContext;
        }
        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            await context.Roles.LoadAsync();
            return context.Roles;
        }
    }
}
 