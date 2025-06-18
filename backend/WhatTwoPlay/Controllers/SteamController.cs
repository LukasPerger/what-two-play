using Microsoft.AspNetCore.Mvc;
using WhatTwoPlay.Util;
using OneOf.Types;
using WhatTwoPlay.Core.Services;

namespace WhatTwoPlay.Controllers;

[Route("/api/steam")]
public sealed class SteamController(ISteamService steamService) : BaseController
{
    // Get single user details
    // Example: GET /api/steam/76561198400371023
    [HttpGet("{userId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<PlayerSummariesResponse>> GetUserDetails(long userId)
    {
        var res = await steamService.GetPlayerSummary(userId);

        return res.Match<ActionResult<PlayerSummariesResponse>>(playerSummariesResponse => Ok(playerSummariesResponse),
                                                                notFound => NotFound("User not found"));
    }

    // Get user friend list
    // Example: GET /api/steam/76561198400371023/friends
    [HttpGet("{userId:long}/friends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<FriendListResponse>> GetFriends(long userId)
    {
        var res = await steamService.GetFriends(userId);
        
        return res.Match<ActionResult<FriendListResponse>>(friendListResponse => Ok(friendListResponse),
                                                           notFound => NotFound("User not found"));
    }

    // Get multiplayer games for both users
    // Example: GET /api/steam/multiplayer/76561198400371023/76561198826421951/apps
    [HttpGet("multiplayer/{userId1:long}/{userId2:long}/apps")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OwnedGamesResponse>> GetMultiplayerGames(long userId1, long userId2)
    {
        try
        {
            var res = await steamService.GetOwnedGames(userId1);
            var res2 = await steamService.GetOwnedGames(userId2);

            if (res.Value is Error)
            {
                return NotFound("User 1 not found");
            }

            if (res2.Value is Error)
            {
                return NotFound("User 2 not found");

            }

            var commonGames = res.AsT0.Response.Games
                                    .Join(res2.AsT0.Response.Games,
                                          g1 => g1.AppId,
                                          g2 => g2.AppId,
                                          (g1, _) => g1)
                                    .ToList();

            var appIds = commonGames
                         .Select(g => g.AppId)
                         .Distinct()
                         .ToList();

            if (!appIds.Any())
            {
                return NotFound("No apps found");
            }

            var gameTags = await GetGameTags(appIds);

            var multiplayerGames = commonGames
                                   .Where(g => gameTags.TryGetValue(g.AppId, out var tags) &&
                                               tags.Contains("Multiplayer"))
                                   .Select(g => g with
                                   {
                                       Tags = gameTags.GetValueOrDefault(g.AppId, new List<string>())
                                                      .ToArray()
                                   })
                                   .ToList();

            return Ok(new OwnedGamesResponse(new OwnedGames(multiplayerGames.Count, multiplayerGames)));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error getting multiplayer games: {ex.Message}");
        }
    }

    private async ValueTask<Dictionary<long, List<string>>> GetGameTags(IEnumerable<long> appIds)
    {
        var dict = await steamService.GetGameTags(appIds);

        return dict;
    }
}
