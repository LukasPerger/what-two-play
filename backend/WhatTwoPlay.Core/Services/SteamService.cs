using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;
using OneOf.Types;
using WhatTwoPlay.Shared;

namespace WhatTwoPlay.Core.Services;

public interface ISteamService
{
    ValueTask<OneOf<PlayerSummariesResponse, NotFound>> GetPlayerSummary(long steamId);
    ValueTask<OneOf<FriendListResponse, NotFound>> GetFriends(long userId);
    ValueTask<OneOf<OwnedGamesResponse, Error>> GetOwnedGames(long userId);
    ValueTask<Dictionary<long, List<string>>> GetGameTags(IEnumerable<long> appIds);
}

internal class SteamService(HttpClient steamApiClient) : ISteamService
{
    private static readonly HttpClient _steamSpyClient = new();

    public async ValueTask<OneOf<PlayerSummariesResponse, NotFound>> GetPlayerSummary(long userId)
    {
        var response = await steamApiClient
            .GetAsync($"ISteamUser/GetPlayerSummaries/v0002/?key={Const.SteamApiKey}&steamids={userId}");

        if (!response.IsSuccessStatusCode)
        {
            return new NotFound();
        }

        var content = await response.Content.ReadAsStringAsync();
        var playerSummaries = JsonSerializer.Deserialize<PlayerSummariesResponse>(content);

        if (playerSummaries?.Response.Players == null || !playerSummaries.Response.Players.Any())
        {
            return new NotFound();
        }

        return playerSummaries;
    }

    public async ValueTask<OneOf<FriendListResponse, NotFound>> GetFriends(long userId)
    {
        var response = await steamApiClient
            .GetAsync($"ISteamUser/GetFriendList/v0001/?key={Const.SteamApiKey}&steamid={userId}&relationship=friend");

        if (!response.IsSuccessStatusCode)
        {
            return new NotFound();
        }

        var content = await response.Content.ReadAsStringAsync();
        var friendList = JsonSerializer.Deserialize<FriendListResponse>(content);

        if (friendList?.FriendsList?.Friends == null || !friendList.FriendsList.Friends.Any())
        {
            return new FriendListResponse(new FriendsList([]));
        }

        var friendIds = string.Join(",", friendList.FriendsList.Friends.Select(f => f.SteamId));
        var playerSummariesResponse = await steamApiClient
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

        return new FriendListResponse(new FriendsList(enrichedFriends));
    }

    public async ValueTask<OneOf<OwnedGamesResponse, Error>> GetOwnedGames(long userId)
    {
        var response = await steamApiClient
            .GetAsync($"IPlayerService/GetOwnedGames/v0001/?key={Const.SteamApiKey}&steamId={userId}&include_appinfo=true");

        if (!response.IsSuccessStatusCode)
        {
            return new Error();
        }

        var content = await response.Content.ReadAsStringAsync();

        var games = JsonSerializer.Deserialize<OwnedGamesResponse>(content);

        if (games == null)
        {
            return new Error();
        }

        return games;
    }

    public async ValueTask<Dictionary<long, List<string>>> GetGameTags(IEnumerable<long> appIds)
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
}

// DTOs
public record FriendListResponse(
    [property: JsonPropertyName("friendslist")]
    FriendsList FriendsList);

public record FriendsList(
    [property: JsonPropertyName("friends")]
    List<FriendResponse> Friends);

public record FriendResponse(
    [property: JsonPropertyName("steamid")]
    long SteamId,
    [property: JsonPropertyName("relationship")]
    string Relationship,
    [property: JsonPropertyName("friend_since")]
    int FriendSince,
    [property: JsonPropertyName("persona_name")]
    string PersonaName,
    [property: JsonPropertyName("avatar_full")]
    string AvatarUrl);

public record AppDetailsResponse(
    [property: JsonPropertyName("data")] AppDetails Data);

public record AppDetails(
    [property: JsonPropertyName("categories")]
    List<Category> Categories,
    [property: JsonPropertyName("genres")] List<Genre> Genres);

public record Category(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("description")]
    string Description);

public record Genre(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("description")]
    string Description);

public record OwnedGamesResponse(
    [property: JsonPropertyName("response")]
    OwnedGames Response);

public record OwnedGames(
    [property: JsonPropertyName("game_count")]
    int GameCount,
    [property: JsonPropertyName("games")] List<OwnedGame> Games);

public record OwnedGame(
    [property: JsonPropertyName("appid")] long AppId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("playtime_forever")]
    int PlaytimeForever,
    [property: JsonPropertyName("img_icon_url")]
    string ImgIconUrl,
    [property: JsonPropertyName("has_community_visible_stats")]
    bool HasCommunityVisibleStats,
    [property: JsonPropertyName("playtime_windows_forever")]
    int PlaytimeWindowsForever,
    [property: JsonPropertyName("playtime_mac_forever")]
    int PlaytimeMacForever,
    [property: JsonPropertyName("playtime_linux_forever")]
    int PlaytimeLinuxForever,
    [property: JsonPropertyName("tags")] string[]? Tags = null);

public record PlayerSummariesResponse(
    [property: JsonPropertyName("response")]
    PlayerSummariesList Response);

public record PlayerSummariesList(
    [property: JsonPropertyName("players")]
    List<PlayerSummary> Players);

public record PlayerSummary(
    [property: JsonPropertyName("steamid")]
    long SteamId,
    [property: JsonPropertyName("personaname")]
    string PersonaName,
    [property: JsonPropertyName("avatarfull")]
    string AvatarFull);
