using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MusicPlayer.Client.Extensions;

internal static class NavigationManagerExtensions
{
    internal static bool IsActive(this NavigationManager target, string href, NavLinkMatch navLinkMatch = NavLinkMatch.Prefix)
    {
        var relativePath = target.ToBaseRelativePath(target.Uri).ToLower();
        return navLinkMatch == NavLinkMatch.All ? relativePath == href.ToLower() : relativePath.StartsWith(href.ToLower());
    }
}
