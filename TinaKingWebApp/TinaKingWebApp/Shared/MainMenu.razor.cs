namespace TinaKingWebApp.Shared
{
    public partial class MainMenu
    {
        private bool _drawerOpen = true;
        void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}
