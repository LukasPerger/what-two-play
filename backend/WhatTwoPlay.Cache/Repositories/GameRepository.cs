using System.Text.Json;
using NRedisStack;
using StackExchange.Redis;
using WhatTwoPlay.Cache.Model;
using WhatTwoPlay.Cache.Util;

namespace WhatTwoPlay.Cache.Repositories;

public interface IGameRepository
{
    ValueTask<Game?> GetGame(long gameId);
    ValueTask SaveGame(Game game);
    ValueTask<IReadOnlyCollection<Game>> GetGames(params IEnumerable<long> gameIds);
}

public sealed class GameRepository(IJsonCommandsAsync json) : IGameRepository
{
    public async ValueTask<Game?> GetGame(long gameId)
    {
        RedisKey key = CacheKeys.GetGameKey(gameId);
        Game? game = await json.GetAsync<Game>(key);

        return game;
    }

    public async ValueTask SaveGame(Game game)
    {
        RedisKey key = CacheKeys.GetGameKey(game.Id);
        string rawJson = JsonSerializer.Serialize(game);

        await json.SetAsync(key, "$", rawJson);
    }

    public async ValueTask<IReadOnlyCollection<Game>> GetGames(params IEnumerable<long> gameIds)
    {
        RedisResult[] res = await json.MGetAsync(gameIds.Select(CacheKeys.GetGameKey).ToArray(), "$");

        var games = res
            .SelectMany(r =>
            {
                try
                {
                    var gameArray = JsonSerializer.Deserialize<Game[]>(r.ToString());

                    return gameArray ?? Enumerable.Empty<Game>();
                }
                catch
                {
                    return [];
                }
            });
        
        return games.ToList();
    }
}
