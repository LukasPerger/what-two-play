using Microsoft.Extensions.DependencyInjection;

namespace WhatTwoPlay.Core.Util;

public static class CoreSetup
{
    public static void ConfigureCore(this IServiceCollection services)
    {
        services.AddSingleton<IClock>(SystemClock.Instance);
        services.AddSingleton<HttpClient>(new HttpClient()
        {
            BaseAddress = new Uri(Settings.)
        });
    }
}
