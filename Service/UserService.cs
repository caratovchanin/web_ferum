using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using webFerum.Models;
using webFerum.Models.AppContext;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace webFerum.Service
{
    public class UserService
    {
        private IDistributedCache cache;
        private WebFerumContext context;
        
        public UserService(WebFerumContext ferrumContext, IDistributedCache distributedCache)
        {
            cache = distributedCache;
            context = ferrumContext;
        }

        public async Task<User?> GetEmployeeAsync(UserModel data)
        {
            User? user = null;

            string? cacheResult = await cache.GetStringAsync(data.Email);
            if (cacheResult is null)
            {
                await context.Users.LoadAsync();
                await context.Roles.LoadAsync();

                user = await context.Users.Where(user => user.Email == data.Email && user.Password == data.Password).FirstAsync();

                UserChached cached = new UserChached { Id = user.Id, Password = user.Password };

                await cache.SetStringAsync(data.Email, JsonSerializer.Serialize(cached), new
                DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
            else
            {
                UserChached cached = JsonSerializer.Deserialize<UserChached?>(cacheResult);
                if (cached.Password == data.Password)
                {
                    await context.Users.LoadAsync();
                    await context.Roles.LoadAsync();
                    user = await context.Users.FindAsync(cached.Id);
                }
            }

            return user;
        }

        public async Task<User?> GetEmployeeAsync(int id)
        {
            User? user = null;

            await context.Users.LoadAsync();
            await context.Roles.LoadAsync();

            user = await context.Users.FindAsync(id);

            UserChached cached = new UserChached { Id = user.Id, Password = user.Password };

            await cache.SetStringAsync(user.Email, JsonSerializer.Serialize(cached), new
            DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return user;
        }

        public async Task<IEnumerable<User>?> GetEmployesAsync()
        {
            await context.Users.LoadAsync();
            await context.Roles.LoadAsync();
            return context.Users;
        }

        public async Task<int> AddUserAsync(UserModel data)
        {
            try
            {
                User user = data.GetUser();

                context.Users.Add(user);
                await context.SaveChangesAsync();
                await context.Users.LoadAsync();

                int id = context.Users.Where(user=>user.Email == data.Email).First().Id;

                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"add user err: {ex.Message}");
            }

            return 0;
        }

        public async Task<int> UpdateUserAsync(User nUser)
        {
            try
            {
                await context.Users.LoadAsync();

                User uUser = await context.Users.FindAsync(nUser.Id);

                uUser.Name = nUser.Name;
                uUser.Surname = nUser.Surname;
                uUser.Lastname = nUser.Lastname;
                uUser.Number = nUser.Number;
                uUser.RoleId = nUser.RoleId;
                uUser.Email = nUser.Email;
                uUser.Password = Utils.Encrypt.sha256(nUser.Password);

                await context.SaveChangesAsync();
                await context.Articles.LoadAsync();

                return nUser.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"update article err: {ex.Message}");
            }
            return 0;
        }
    }
}
