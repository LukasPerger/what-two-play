using System.Runtime.CompilerServices;

namespace WhatTwoPlay.Core.Util;

public sealed class Settings
{
    public const string SectionKey = "General";
    private int CookieExpiration { get; set; }
    public required string ClientOrigin { get; init; }
    public TimeSpan CookieTimeSpan => TimeSpan.FromHours(CookieExpiration);
}
