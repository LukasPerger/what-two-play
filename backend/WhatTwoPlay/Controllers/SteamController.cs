using Microsoft.AspNetCore.Mvc;
using WhatTwoPlay.Shared;
using WhatTwoPlay.Util;
using System.Text.Json;
using WhatTwoPlay.Core.Services;

namespace WhatTwoPlay.Controllers;

[Route("/api/steam")]
public sealed class SteamController(
    HttpClient steamApiClient) : BaseController
{
    private readonly HttpClient _steamApiClient = steamApiClient;

    // Get single user details
    // Example: GET /api/steam/76561198400371023
    [HttpGet("{userId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<PlayerSummariesResponse>> GetUserDetails(long userId)
    {
        var response = await _steamApiClient
            .GetAsync($"ISteamUser/GetPlayerSummaries/v0002/?key={Const.SteamApiKey}&steamids={userId}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("User not found");
        }

        var content = await response.Content.ReadAsStringAsync();
        var playerSummaries = JsonSerializer.Deserialize<PlayerSummariesResponse>(content);

        if (playerSummaries?.Response.Players == null || !playerSummaries.Response.Players.Any())
        {
            return NotFound("User not found");
        }

        return Ok(playerSummaries);
    }

    // Get user friend list
    // Example: GET /api/steam/76561198400371023/friends
    [HttpGet("{userId:long}/friends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<FriendListResponse>> GetFriends(long userId)
    {
        var response = await _steamApiClient
            .GetAsync($"ISteamUser/GetFriendList/v0001/?key={Const.SteamApiKey}&steamid={userId}&relationship=friend");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("User not found");
        }

        var content = await response.Content.ReadAsStringAsync();
        var friendList = JsonSerializer.Deserialize<FriendListResponse>(content);

        if (friendList?.FriendsList?.Friends == null || !friendList.FriendsList.Friends.Any())
        {
            return Ok(new List<FriendResponse>());
        }

        var friendIds = string.Join(",", friendList.FriendsList.Friends.Select(f => f.SteamId));
        var playerSummariesResponse = await _steamApiClient
            .GetAsync($"ISteamUser/GetPlayerSummaries/v0002/?key={Const.SteamApiKey}&steamids={friendIds}");

        var summariesContent = await playerSummariesResponse.Content.ReadAsStringAsync();
        var playerSummaries = JsonSerializer.Deserialize<PlayerSummariesResponse>(summariesContent);

        var enrichedFriends = friendList.FriendsList.Friends
                                        .Join(playerSummaries?.Response.Players ?? new List<PlayerSummary>(),
                                              f => f.SteamId,
                                              p => p.SteamId,
                                              (f, p) => new FriendResponse(f.SteamId,
                                                                           f.Relationship,
                                                                           f.FriendSince,
                                                                           p.PersonaName,
                                                                           p.AvatarFull))
                                        .ToList();

        return Ok(new FriendListResponse(new FriendsList(enrichedFriends)));
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
            var (games1Task, games2Task) = (
                GetUserGames(userId1),
                GetUserGames(userId2)
            );

            var games1 = await games1Task;
            var games2 = await games2Task;

            if (games1 is null)
            {
                return NotFound("User 1 not found");
            }

            if (games2 is null)
            {
                return NotFound("User 2 not found");
            }

            var commonGames = games1.Response.Games
                                    .Join(games2.Response.Games,
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
                .Select(g => g with { Tags = gameTags.GetValueOrDefault(g.AppId, new List<string>()).ToArray() })
                .ToList();

            return Ok(new OwnedGamesResponse(new OwnedGames(multiplayerGames.Count, multiplayerGames)));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error getting multiplayer games: {ex.Message}");
        }
    }

    private static readonly HttpClient _steamSpyClient = new();

    private async ValueTask<Dictionary<long, List<string>>> GetGameTags(IEnumerable<long> appIds)
    {
        var result = new Dictionary<long, List<string>>();

        foreach (var appId in appIds)
        {
            var url = $"https://steamspy.com/api.php?request=appdetails&appid={appId}";

            try
            {
                var response = await _steamSpyClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("tags", out var tagsElement))
                {
                    var tags = tagsElement.EnumerateObject().Select(tag => tag.Name).ToList();

                    result[appId] = tags;
                }
                else
                {
                    result[appId] = new List<string>(); // No tags found
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tags for AppID {appId}: {ex.Message}");
                result[appId] = new List<string>(); // Fail gracefully
            }
        }

        return result;
    }

    private async Task<OwnedGamesResponse?> GetUserGames(long userId)
    {
        var response = await _steamApiClient
            .GetAsync($"IPlayerService/GetOwnedGames/v0001/?key={Const.SteamApiKey}&steamId={userId}&include_appinfo=true");

        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<OwnedGamesResponse>(content);
    }
}