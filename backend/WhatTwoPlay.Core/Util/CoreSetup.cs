using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WhatTwoPlay.Core.Services;

namespace WhatTwoPlay.Core.Util;

public static class CoreSetup
{
    public static void ConfigureCore(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddSingleton<IClock>(SystemClock.Instance);

        var baseApi = configuration.GetSection(Settings.SectionKey)["SteamBaseApi"];

        if (string.IsNullOrEmpty(baseApi))
        {
            throw new InvalidOperationException("SteamBaseApi has to be configured in the settings");
        }

        services.AddSingleton<HttpClient>(new HttpClient()
        {
            BaseAddress = new Uri(baseApi)
        });
        services.AddScoped<ISteamService, SteamService>();
        services.AddScoped<ICacheService, CacheService>();
    }
}
