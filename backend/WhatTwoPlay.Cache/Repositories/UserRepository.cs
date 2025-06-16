using NRedisStack;
using WhatTwoPlay.Cache.Model;
using WhatTwoPlay.Cache.Util;

namespace WhatTwoPlay.Cache.Repositories;

public interface IUserRepository
{
    User? GetUser(string userId);
    void SaveUser(User user);
}

internal sealed class UserRepository(IJsonCommands json) : IUserRepository
{
    public User? GetUser(string userId)
    {
        string cacheKey = CacheKeys.GetUserKey(userId);

        var cachedItem = json.Get<User>(cacheKey);

        return cachedItem;
    }

    public void SaveUser(User user)
    {
        var key = CacheKeys.GetUserKey(user.Id);
        json.Set(key, "$", user);
    }
}
