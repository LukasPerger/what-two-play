using StackExchange.Redis;

namespace WhatTwoPlay.Cache.Util;

public static class CacheKeys
{
    private const char Seperator = ':';

    private const string UserPrefix = "user";
    private const string GamePrefix = "game";
    private const string TagPrefix = "tag";
    
    public static RedisKey GetUserKey(long userId) => string.Join(Seperator, UserPrefix, userId);
    public static RedisKey GetGameKey(long gameId) => string.Join(Seperator, GamePrefix, gameId);
    public static RedisKey GetTagKey(long categoryId) => string.Join(Seperator, TagPrefix, categoryId);
}
