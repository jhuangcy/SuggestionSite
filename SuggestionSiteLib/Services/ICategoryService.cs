using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionSiteLib.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> FindAllAsync();
        Task InsertOneAsync(Category category);
    }

    public class CategoryService : ICategoryService
    {
        private const string CACHE_NAME = "Categories";
        private readonly IMemoryCache _cache;
        private readonly IMongoCollection<Category> _categories;

        public CategoryService(IMemoryCache cache, IDbConfig db)
        {
            _cache = cache;
            _categories = db.Categories;
        }

        public async Task<List<Category>> FindAllAsync()
        {
            var res = _cache.Get<List<Category>>(CACHE_NAME) ?? await _categories.FindAsync(_ => true).Result.ToListAsync();
            _cache.Set(CACHE_NAME, res, TimeSpan.FromDays(1));
            return res;
        }

        public async Task InsertOneAsync(Category category) => await _categories.InsertOneAsync(category);
    }
}
