using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MusicPlayer.Client.Extensions;

namespace MusicPlayer.Client.Shared;

public partial class MenuItem : IDisposable
{
    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public string? ImageUrlActive { get;set; }
    [Parameter]
    public string? ImageUrlInactive { get;set; }
    [Parameter]
    public string? Name { get;set; }
    [Parameter]
    public string? NavRoute { get;set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? InputAttributes { get; set; }

    private string ImageUrl => (Active ? ImageUrlActive : ImageUrlInactive) ?? "";
    private bool Active => NavRoute != null && NavigationManager.IsActive(NavRoute, string.IsNullOrWhiteSpace(NavRoute) ? NavLinkMatch.All : NavLinkMatch.Prefix);

    private string NameClassCSS => Active ? "text-white" : "";

    protected override void OnInitialized() => NavigationManager.LocationChanged += (s, e) => StateHasChanged();

    public void Dispose()
    {
        NavigationManager.LocationChanged -= (s, e) => StateHasChanged();
    }
}