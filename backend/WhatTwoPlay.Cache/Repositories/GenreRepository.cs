using System.Text.Json;
using NRedisStack;
using StackExchange.Redis;
using WhatTwoPlay.Cache.Model;
using WhatTwoPlay.Cache.Util;

namespace WhatTwoPlay.Cache.Repositories;

public interface IGenreRepository
{
    ValueTask<Genre?> GetGenre(long genreId);
    ValueTask SaveGenre(Genre genre);
    ValueTask<IReadOnlyCollection<Genre>> GetGenres(params IEnumerable<long> genreIds);
}

public sealed class GenreRepository(IJsonCommandsAsync json) : IGenreRepository
{
    public async ValueTask<Genre?> GetGenre(long genreId)
    {
        RedisKey key = CacheKeys.GetGenreKey(genreId);
        Genre? genre = await json.GetAsync<Genre?>(key);

        return genre;
    }

    public async ValueTask SaveGenre(Genre genre)
    {
        RedisKey key = CacheKeys.GetGenreKey(genre.Id);
        string rawJson = JsonSerializer.Serialize(genre);

        await json.SetAsync(key, "$", rawJson);
    }

    public async ValueTask<IReadOnlyCollection<Genre>> GetGenres(params IEnumerable<long> genreIds)
    {
        RedisResult[] res = await json.MGetAsync(genreIds.Select(CacheKeys.GetCategoryKey).ToArray(), "$");

        var genres = res
            .SelectMany(r =>
            {
                try
                {
                    var genreArray = JsonSerializer.Deserialize<Genre[]>(r.ToString());

                    return genreArray ?? Enumerable.Empty<Genre>();
                }
                catch
                {
                    return [];
                }
            });
        
        return genres.ToList();
    }
}
