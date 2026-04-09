using MenuAPI;
using CitizenFX.Core;
using static vMenuShared.PermissionsManager;
using static CitizenFX.Core.Native.API;

namespace vMenuClient.menus
{
    public class ModerationOptions
    {
        private Menu Menu;
        public static BannedPlayers BannedPlayersMenu { get; set;}


        private static void AddMenu(Menu parentMenu, Menu submenu, MenuItem menuButton)
        {
            parentMenu.AddMenuItem(menuButton);
            MenuController.AddSubmenu(parentMenu, submenu);
            MenuController.BindMenuItem(parentMenu, submenu, menuButton);
            submenu.RefreshIndex();
        }

        private void CreateMenu()
        {
            Menu = new Menu(Game.Player.Name, "Moderation Options");


            if (IsAllowed(Permission.OPUnban) || IsAllowed(Permission.OPViewBannedPlayers))
            {
                BannedPlayersMenu = new BannedPlayers();
                var menu = BannedPlayersMenu.GetMenu();
                var button = new MenuItem("Banned Players", "View and manage all banned players in this menu.")
                {
                    Label = "→→→"
                };
                AddMenu(Menu, menu, button);

                Menu.OnItemSelect += (sender, item, index) =>
                {
                    if (item == button)
                    {
                        BaseScript.TriggerServerEvent("vMenu:RequestBanList", Game.Player.Handle);
                        menu.RefreshIndex();
                    }
                };
            }

            if (IsAllowed(Permission.MDViewReports) && IsAllowed(Permission.MDViewReportStats)) {
                var menu = new Menu(Game.Player.Name, "View Player Reports");
                var button = new MenuItem("Player Reports", "View all player reports in this menu.")
                {
                    Label = "→→→"
                };
                AddMenu(Menu, menu, button);
            }
            else if (IsAllowed(Permission.MDViewReportStats))
            {
                var menu = new Menu(Game.Player.Name, "Moderation Report Statstics ");
                var button = new MenuItem("Moderator Statistics", "View how many reports moderators have handled during their current session.")
                {
                    Label = "→→→"
                };
                AddMenu(Menu, menu, button);
            }
        }

        public Menu GetMenu()
        {
            if (Menu == null)
            {
                CreateMenu();
            }
            return Menu;
        }
    }
}
