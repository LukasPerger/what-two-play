using System.Runtime.CompilerServices;

namespace WhatTwoPlay.Core.Util;

public sealed class Settings
{
    public const string SectionKey = "General";
    private int CookieExpiration { get; set; }
    public required string ClientOrigin { get; init; }
    public TimeSpan CookieTimeSpan => TimeSpan.FromHours(CookieExpiration);
    public required string SteamBaseApi { get; init; }
    public required string GetPlayerSummaries { get; init; }
    public required string GetOwnedGames { get; init; }
    public required string GetAppList { get; init; }
}
