using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using webFerum.Models.AppContext;

namespace webFerum.Service
{
    public class ItemService
    {
        private IDistributedCache cache;
        private WebFerumContext context;
        public ItemService(WebFerumContext ferrumContext, IDistributedCache distributedCache)
        {
            cache = distributedCache;
            context = ferrumContext;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            await context.Items.LoadAsync();
            return context.Items;
        }

        public async Task<IQueryable<Item>> GetItemsIQueryableAsync()
        {
            await context.Items.LoadAsync();
            return context.Items.ToList<Item>().AsQueryable<Item>();
        }

        public async Task<Item> GetItemAsync(int id)
        {
            await context.Articles.LoadAsync();

            Item item = await context.Items.FindAsync(id);

            return item;
        }

        public async Task<int> AddItemAsync(Item item)
        {
            try
            {
                context.Items.Add(item);

                await context.SaveChangesAsync();
                await context.Items.LoadAsync();

                int id = context.Items.Where(it => it.Name == item.Name).First().Id;

                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"add item err: {ex.Message}");
            }
            return 0;
        }

        public async Task<int> UpdateItemAsync(Item nItem)
        {
            try
            {
                await context.Items.LoadAsync();

                Item updatedItem = await context.Items.FindAsync(nItem.Id);

                updatedItem.Description = nItem.Description;
                updatedItem.Cost = nItem.Cost;
                updatedItem.Name = nItem.Name;
                await context.SaveChangesAsync();
                await context.Items.LoadAsync();

                return updatedItem.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"update item err: {ex.Message}");
            }
            return 0;
        }
    }
}
