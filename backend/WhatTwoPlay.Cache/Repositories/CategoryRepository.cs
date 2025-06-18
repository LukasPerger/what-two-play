using System.Text.Json;
using NRedisStack;
using StackExchange.Redis;
using WhatTwoPlay.Cache.Model;
using WhatTwoPlay.Cache.Util;

namespace WhatTwoPlay.Cache.Repositories;

public interface ICategoryRepository
{
    ValueTask<Category?> GetCategory(long categoryId);
    ValueTask SaveCategory(Category category);
    ValueTask<IReadOnlyCollection<Category>> GetCategories(params IEnumerable<long> categoryIds);
}

public sealed class CategoryRepository(IJsonCommandsAsync json) : ICategoryRepository
{
    public async ValueTask<Category?> GetCategory(long categoryId)
    {
        RedisKey key = CacheKeys.GetCategoryKey(categoryId);
        Category? category = await json.GetAsync<Category?>(key);
        
        return category;
    }

    public async ValueTask SaveCategory(Category category)
    {
        RedisKey key = CacheKeys.GetCategoryKey(category.Id);
        var rawJson = JsonSerializer.Serialize(category);
        
        await json.SetAsync(key, "$", rawJson);
    }

    public async ValueTask<IReadOnlyCollection<Category>> GetCategories(params IEnumerable<long> categoryIds)
    {
        RedisResult[] res = await json.MGetAsync(categoryIds.Select(CacheKeys.GetCategoryKey).ToArray(), "$");
        
        var categories = res
            .SelectMany(r =>
            {
                try
                {
                    var categoryArray = JsonSerializer.Deserialize<Category[]>(r.ToString());

                    return categoryArray ?? Enumerable.Empty<Category>();
                }
                catch
                {
                    return [];
                }
            });
        
        return categories.ToList();
    }
}
