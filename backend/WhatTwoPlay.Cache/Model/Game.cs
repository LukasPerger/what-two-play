namespace WhatTwoPlay.Cache.Model;

public class Game
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string ImageUrl { get; set; }
    public required List<string> Tags { get; set; }
}