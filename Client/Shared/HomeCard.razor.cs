using Microsoft.AspNetCore.Components;

namespace MusicPlayer.Client.Shared;

public partial class HomeCard
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? InputAttributes { get; set; }
    [Parameter]
    public string? Title { get;set; }
    [Parameter]
    public string? SubTitle { get;set; }
    [Parameter]
    public string? ImageUrl { get;set; }
}