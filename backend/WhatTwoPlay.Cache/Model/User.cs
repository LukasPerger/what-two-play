namespace WhatTwoPlay.Cache.Model;

public class User
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string ProfilePictureUrl { get; set; }
    public required List<long> GameAppIds { get; set; }
}
