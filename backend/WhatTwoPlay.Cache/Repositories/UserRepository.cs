using System.Text.Json;
using NRedisStack;
using StackExchange.Redis;
using WhatTwoPlay.Cache.Model;
using WhatTwoPlay.Cache.Util;

namespace WhatTwoPlay.Cache.Repositories;

public interface IUserRepository
{
    ValueTask<User?> GetUser(long userId);
    ValueTask SaveUser(User user);
    ValueTask<IReadOnlyCollection<User>> GetUsers(params IEnumerable<long> userIds);
}

internal sealed class UserRepository(IJsonCommandsAsync json) : IUserRepository
{
    public async ValueTask<User?> GetUser(long userId)
    {
        RedisKey cacheKey = CacheKeys.GetUserKey(userId);

        var cachedItem = await json.GetAsync<User>(cacheKey);

        return cachedItem;
    }

    public async ValueTask SaveUser(User user)
    {
        var key = CacheKeys.GetUserKey(user.Id);
        string rawJson = JsonSerializer.Serialize(user);
        
        await json.SetAsync(key, "$", rawJson);
    }

    public async ValueTask<IReadOnlyCollection<User>> GetUsers(params IEnumerable<long> userIds)
    {
        RedisResult[] res = await json.MGetAsync(userIds.Select(CacheKeys.GetUserKey).ToArray(), "$");

        var users = res
            .SelectMany(r =>
            {
                try
                {
                    var userArray = JsonSerializer.Deserialize<User[]>(r.ToString());

                    return userArray ?? Enumerable.Empty<User>();
                }
                catch
                {
                    return [];
                }
            });
        
        return users.ToList();
    }
}
