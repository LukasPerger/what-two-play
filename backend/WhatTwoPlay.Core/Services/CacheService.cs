using OneOf;
using OneOf.Types;
using WhatTwoPlay.Cache.Model;
using WhatTwoPlay.Cache.Repositories;

namespace WhatTwoPlay.Core.Services;

public interface ICacheService : ISteamService
{
    ValueTask SaveInfo(PlayerSummariesResponse res, OwnedGamesResponse resOwnedGames, FriendListResponse resFriends);
}

internal class CacheService(IGameRepository gameRepository, IUserRepository userRepository)
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

        var ownedGameTasks = rawGames.Select(g => Task.FromResult(new OwnedGame(
                                                                                g.Id,
                                                                                g.Name,
                                                                                0,
                                                                                g.ImageUrl,
                                                                                true,
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                g.Tags.ToArray()
                                                                               )));

        var games = (await Task.WhenAll(ownedGameTasks)).ToList();

        return new OwnedGamesResponse(new OwnedGames(games.Count, games));
    }

    public async ValueTask<Dictionary<long, List<string>>> GetGameTags(IEnumerable<long> appIds)
    {
        var games = await gameRepository.GetGames(appIds);

        return games.ToDictionary(g => g.Id, g => g.Tags);
    }

    public async ValueTask SaveInfo(PlayerSummariesResponse res, OwnedGamesResponse resOwnedGames, FriendListResponse resFriends)
    {
        var sum = res.Response.Players[0];
        var games = resOwnedGames.Response.Games;
        var friends = resFriends.FriendsList.Friends;

        await userRepository.SaveUser(new User()
        {
            Id = sum.SteamId,
            Name = sum.PersonaName,
            ProfilePictureUrl = sum.AvatarFull,
            GameAppIds = games.Select(g => g.AppId).ToList(),
            FriendIds = friends.Select(f => f.SteamId).ToList()
        });
        
        foreach (var friendResponse in friends)
        {
            await userRepository.SaveUser(new User()
            {
                Id = friendResponse.SteamId,
                Name = friendResponse.PersonaName,
                ProfilePictureUrl = friendResponse.AvatarUrl,
                FriendIds = [],
                GameAppIds = []
            });
        }

        foreach (var game in games)
        {
            await gameRepository.SaveGame(new Game()
            {
                Id = game.AppId,
                Name = game.Name,
                ImageUrl = game.ImgIconUrl,
                Tags = game.Tags?.ToList() ?? []
            });
        }
    }
}
