using StackExchange.Redis;

namespace WhatTwoPlay.Cache.Util;

public static class CacheKeys
{
    private const char Seperator = ':';

    private const string UserPrefix = "user";
    private const string GamePrefix = "game";
    private const string GenrePrefix = "genre";
    private const string CategoryPrefix = "category";
    
    public static RedisKey GetUserKey(string userId) => string.Join(Seperator, UserPrefix, userId);
    public static RedisKey GetGameKey(long gameId) => string.Join(Seperator, GamePrefix, gameId);
    public static RedisKey GetGenreKey(long genreId) => string.Join(Seperator, GenrePrefix, genreId);
    public static RedisKey GetCategoryKey(long categoryId) => string.Join(Seperator, CategoryPrefix, categoryId);
}
