
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WhatTwoPlay.Cache.Util;

public static class CacheSetup
{
    private const string Prefix = "WhatTwoPlay_";

    public static void ConfigureCache(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configurationManager.GetConnectionString("Redis");
            options.InstanceName = Prefix;
        });
    }
}
