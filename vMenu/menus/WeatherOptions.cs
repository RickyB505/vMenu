using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using CitizenFX.Core;

using MenuAPI;

using vMenuShared;

using static vMenuClient.CommonFunctions;
using static vMenuShared.PermissionsManager;

namespace vMenuClient.menus
{
    public class WeatherOptions
    {
        // Variables
        private Menu menu;
        public MenuCheckboxItem dynamicWeatherEnabled;
        public MenuCheckboxItem blackout;
        public MenuCheckboxItem vehicleBlackout;
        public MenuCheckboxItem snowEnabled;

        public readonly List<string> WeatherTypes = new()
        {
            "EXTRASUNNY",
            "CLEAR",
            "NEUTRAL",
            "SMOG",
            "FOGGY",
            "CLOUDS",
            "OVERCAST",
            "CLEARING",
            "RAIN",
            "THUNDER",
            "BLIZZARD",
            "SNOW",
            "SNOWLIGHT",
            "XMAS",
            "HALLOWEEN"
        };
        
        public readonly List<string> WeatherLabels = new()
        {
            "Extra Sunny",
            "Clear",
            "Neutral",
            "Smog",
            "Foggy",
            "Cloudy",
            "Overcast",
            "Clearing",
            "Rainy",
            "Thunder",
            "Blizzard",
            "Snow",
            "Light Snow",
            "X-MAS Snow",
            "Halloween"
        };
        private void CreateMenu()
        {
            // Create the menu.
            menu = new Menu(Game.Player.Name, "Weather Options");

            dynamicWeatherEnabled = new MenuCheckboxItem("Toggle Dynamic Weather", "Enable or disable dynamic weather changes.", EventManager.DynamicWeatherEnabled);
            blackout = new MenuCheckboxItem("Toggle Blackout", "This disables or enables all lights across the map.", EventManager.IsBlackoutEnabled);
            vehicleBlackout = new MenuCheckboxItem("Toggle Vehicle Lights Blackout", "This disables or enables all vehicle lights across the map.", !EventManager.IsVehicleLightsEnabled);
            snowEnabled = new MenuCheckboxItem("Enable Snow Effects", "This will force snow to appear on the ground and enable snow particle effects for peds and vehicles. Combine with X-MAS or Light Snow weather for best results.", ConfigManager.GetSettingsBool(ConfigManager.Setting.vmenu_enable_snow));
            
            var weatherA = new MenuListItem("Weather A", WeatherLabels, 0, "Send to apply Weather A"); // dont forget to fix index cunt
            var weatherB = new MenuListItem("Weather B", WeatherLabels, 0, "Send to apply Weather B"); // dont forget to fix index cunt
            var weatherMix = new MenuSliderItem("Weather Mix", 0, 100, 50, false);

            var removeclouds = new MenuItem("Remove All Clouds", "Remove all clouds from the sky!");
            var randomizeclouds = new MenuItem("Randomize Clouds", "Add random clouds to the sky!");

            if (IsAllowed(Permission.WODynamic))
            {
                menu.AddMenuItem(dynamicWeatherEnabled);
            }
            if (IsAllowed(Permission.WOBlackout))
            {
                menu.AddMenuItem(blackout);
            }
            if (IsAllowed(Permission.WOVehBlackout))
            {
                menu.AddMenuItem(vehicleBlackout);
            }
            if (IsAllowed(Permission.WOSetWeather))
            {
                menu.AddMenuItem(snowEnabled);
    
                menu.AddMenuItem(weatherA);
                menu.AddMenuItem(weatherB);
                menu.AddMenuItem(weatherMix);
            }
            if (IsAllowed(Permission.WORandomizeClouds))
            {
                menu.AddMenuItem(randomizeclouds);
            }

            if (IsAllowed(Permission.WORemoveClouds))
            {
                menu.AddMenuItem(removeclouds);
            }

            menu.OnItemSelect += (sender, item, index2) =>
            {
                if (item == removeclouds)
                {
                    ModifyClouds(true);
                }
                else if (item == randomizeclouds)
                {
                    ModifyClouds(false);
                }
                else if (item == weatherA)
                {
                    Notify.Custom($"The weather will be changed to ~y~{WeatherLabels[weatherA.Index]}~s~. This will take {EventManager.WeatherChangeTime} seconds.");
                    UpdateServerWeather(WeatherTypes[weatherA.Index], EventManager.DynamicWeatherEnabled, EventManager.IsSnowEnabled);
                }
            };

            menu.OnCheckboxChange += (sender, item, index, _checked) =>
            {
                if (item == dynamicWeatherEnabled)
                {
                    Notify.Custom($"Dynamic weather changes are now {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerWeather(EventManager.GetServerWeather, _checked, EventManager.IsSnowEnabled);
                }
                else if (item == blackout)
                {
                    Notify.Custom($"Blackout mode is now {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerBlackout(_checked);
                }
                else if (item == vehicleBlackout)
                {
                    Notify.Custom($"Vehicle light blackout mode is now {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerVehicleBlackout(!_checked);
                }
                else if (item == snowEnabled)
                {
                    if (EventManager.GetServerWeather is "XMAS" or "SNOWLIGHT" or "SNOW" or "BLIZZARD")
                    {
                        Notify.Custom($"Snow effects cannot be disabled when weather is ~y~{EventManager.GetServerWeather}~s~.");
                        return;
                    }

                    Notify.Custom($"Snow effects will now be forced {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerWeather(EventManager.GetServerWeather, EventManager.DynamicWeatherEnabled, _checked);
                }
            };
        }



        /// <summary>
        /// Create the menu if it doesn't exist, and then returns it.
        /// </summary>
        /// <returns>The Menu</returns>
        public Menu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }
    }
}
