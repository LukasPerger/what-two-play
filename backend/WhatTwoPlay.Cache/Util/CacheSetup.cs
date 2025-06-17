
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace WhatTwoPlay.Cache.Util;

public static class CacheSetup
{
    public static void ConfigureCache(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        var redisConfig = configurationManager.GetConnectionString("Redis");

        if (string.IsNullOrWhiteSpace(redisConfig))
        {
            throw new InvalidOperationException("Redis connection string has to be configured");
        }
        
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConfig));
        services.AddSingleton<IDatabase>(sp =>
        {
            IConnectionMultiplexer connection = sp.GetRequiredService<IConnectionMultiplexer>();
            IDatabase db = connection.GetDatabase();

            return db;
        });
        services.AddScoped<IJsonCommandsAsync>(sp =>
        {
            IDatabase db = sp.GetRequiredService<IDatabase>();

            return db.JSON();
        });
    }
}
