namespace MusicPlayer.Client.Shared;

public partial class TopMenu
{
    private bool menuIsOpen = false;

    /// <summary>
    ///  Background color for menu button
    /// </summary>
    private string BackgroundColorCSS => menuIsOpen ? "dark2" : "black";

    private void ToggleMenu()
    {
        menuIsOpen = !menuIsOpen;
    }
}
