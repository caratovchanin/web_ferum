using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using webFerum.Models.AppContext;

namespace webFerum.Service
{
    public class ArticleService
    {
        private IDistributedCache cache;
        private WebFerumContext context;

        public ArticleService(WebFerumContext ferrumContext, IDistributedCache distributedCache)
        {
            cache = distributedCache;
            context = ferrumContext;
        }

        public async Task<IEnumerable<Article>> GetAtriclesAsync()
        {
            await context.Articles.LoadAsync();
            return context.Articles;
        }
        public async Task<IQueryable<Article>> GetAtriclesIQueryableAsync()
        {
            await context.Articles.LoadAsync();
            return context.Articles.ToList<Article>().AsQueryable<Article>();
        }
        public async Task<Article> GetAtricleAsync(int id)
        {
            await context.Articles.LoadAsync();

            Article article = await context.Articles.FindAsync(id);

            return article;
        }
        public async Task<int> AddArticleAsync(Article art)
        {
            try
            {
                context.Articles.Add(art);

                await context.SaveChangesAsync();
                await context.Articles.LoadAsync();

                int id = context.Articles.Where(article => article.Head == art.Head ).First().Id;
            
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"add article err: {ex.Message}");
            }
            return 0;
        }

        public async Task<int> UpdateArticleAsync(Article newArt)
        {
            try
            {
                await context.Articles.LoadAsync();

                Article updatedArticle = await context.Articles.FindAsync(newArt.Id);

                updatedArticle.Description = newArt.Description;
                updatedArticle.Text = newArt.Text;
                updatedArticle.Face = newArt.Face;

                await context.SaveChangesAsync();
                await context.Articles.LoadAsync();

                return updatedArticle.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"update article err: {ex.Message}");
            }
            return 0;
        }
    }
}
