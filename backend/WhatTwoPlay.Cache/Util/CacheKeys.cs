namespace WhatTwoPlay.Cache.Util;

public static class CacheKeys
{
    private const char Seperator = ':';

    private const string UserPrefix = "user";
    private const string GamePrefix = "game";
    
    public static string GetUserKey(string userId) => string.Join(Seperator, UserPrefix, userId);
    public static string GetGameKey(string gameId) => string.Join(Seperator, GamePrefix, gameId);
}
