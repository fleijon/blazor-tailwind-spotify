using Microsoft.AspNetCore.Components;

namespace MusicPlayer.Client.Shared;

public partial class CategorySelect
{
    [Parameter]
    public string? Category { get;set; }
    [Parameter]
    public string? ImageUrl { get;set; }

    private readonly string backgroundColor = $"rgb({RandInt()},{RandInt()},{RandInt()})";

    private static readonly Random _random = new();
    private static int RandInt() => _random.Next(1, 255);
}