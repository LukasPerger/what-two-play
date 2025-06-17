namespace WhatTwoPlay.Shared;

public static class Const
{
    public static readonly DateTimeZone TimeZone = DateTimeZoneProviders.Tzdb["Europe/Vienna"];

    public static string SteamApiKey
    {
        get 
        {
            var key = Environment.GetEnvironmentVariable("STEAM_API_KEY");

            if (key == null)
            {
                throw new InvalidOperationException("Steam api key was not set in an environment file");
            }

            return key;
        }
    }
}
