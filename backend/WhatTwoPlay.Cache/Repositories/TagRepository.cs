using System.Text.Json;
using NRedisStack;
using StackExchange.Redis;
using WhatTwoPlay.Cache.Model;
using WhatTwoPlay.Cache.Util;

namespace WhatTwoPlay.Cache.Repositories;

public interface ITagRepository
{
    ValueTask<Tag?> GetTag(long categoryId);
    ValueTask SaveTag(Tag tag);
    ValueTask<IReadOnlyCollection<Tag>> GetTags(params IEnumerable<long> categoryIds);
}

public sealed class TagRepository(IJsonCommandsAsync json) : ITagRepository
{
    public async ValueTask<Tag?> GetTag(long categoryId)
    {
        RedisKey key = CacheKeys.GetTagKey(categoryId);
        Tag? category = await json.GetAsync<Tag?>(key);
        
        return category;
    }

    public async ValueTask SaveTag(Tag tag)
    {
        RedisKey key = CacheKeys.GetTagKey(tag.Id);
        var rawJson = JsonSerializer.Serialize(tag);
        
        await json.SetAsync(key, "$", rawJson);
    }

    public async ValueTask<IReadOnlyCollection<Tag>> GetTags(params IEnumerable<long> categoryIds)
    {
        RedisResult[] res = await json.MGetAsync(categoryIds.Select(CacheKeys.GetTagKey).ToArray(), "$");
        
        var categories = res
            .SelectMany(r =>
            {
                try
                {
                    var categoryArray = JsonSerializer.Deserialize<Tag[]>(r.ToString());

                    return categoryArray ?? Enumerable.Empty<Tag>();
                }
                catch
                {
                    return [];
                }
            });
        
        return categories.ToList();
    }
}
