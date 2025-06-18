using OneOf;
using OneOf.Types;
using WhatTwoPlay.Cache.Repositories;

namespace WhatTwoPlay.Core.Services;

public interface ICacheService : ISteamService
{
    
}

internal class CacheService(IGameRepository gameRepository, IUserRepository userRepository, TagRepository tagRepository)
    : ICacheService
{
    public async ValueTask<OneOf<PlayerSummariesResponse, NotFound>> GetPlayerSummary(long steamId)
    {
        var user = await userRepository.GetUser(steamId);

        if (user == null)
        {
            return new NotFound();
        }

        return new PlayerSummariesResponse(new PlayerSummariesList([
            new PlayerSummary(user.Id, user.Name, user.ProfilePictureUrl)
        ]));
    }

    public async ValueTask<OneOf<FriendListResponse, NotFound>> GetFriends(long userId)
    {
        var user = await userRepository.GetUser(userId);

        if (user == null)
        {
            return new NotFound();
        }

        var friends = await userRepository.GetUsers(user.FriendIds);

        return new FriendListResponse(new FriendsList(friends
                                                      .Select(f => new FriendResponse(f.Id, string.Empty, 0, f.Name,
                                                               f.ProfilePictureUrl)).ToList()));
    }

    public async ValueTask<OneOf<OwnedGamesResponse, Error>> GetOwnedGames(long userId)
    {
        var user = await userRepository.GetUser(userId);

        if (user == null)
        {
            return new Error();
        }

        var rawGames = await gameRepository.GetGames(user.GameAppIds);

        var ownedGameTasks = rawGames.Select(async g =>
        {
            var tags = await tagRepository.GetTags(g.GenreIds);
            return new OwnedGame(
                                 g.Id,
                                 g.Name,
                                 0,
                                 g.ImageUrl,
                                 true,
                                 0,
                                 0,
                                 0,
                                 tags.Select(t => t.Name).ToArray()
                                );
        });

        var games = (await Task.WhenAll(ownedGameTasks)).ToList();

        return new OwnedGamesResponse(new OwnedGames(games.Count, games));
    }

    public async ValueTask<Dictionary<long, List<string>>> GetGameTags(IEnumerable<long> appIds)
    {
        var rawGames = await gameRepository.GetGames(appIds);

        var ownedGameTasks = rawGames.Select(async g =>
        {
            var tags = await tagRepository.GetTags(g.GenreIds);
            return new OwnedGame(
                                 g.Id,
                                 g.Name,
                                 0,
                                 g.ImageUrl,
                                 true,
                                 0,
                                 0,
                                 0,
                                 tags.Select(t => t.Name).ToArray()
                                );
        });

        var games = (await Task.WhenAll(ownedGameTasks)).ToList();
        
        return games.ToDictionary(g => g.AppId, g => g.Tags?.ToList() ?? []);
    }
}
