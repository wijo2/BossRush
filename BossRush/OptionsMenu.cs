using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossRush
{
    public static class OptionsMenu
    {
        public static GUIBox gui;
        public static bool hasInit = false;

        public static ToggleOption[] weapons = new ToggleOption[]
        {
            new ToggleOption("sword", 100, true),
            new ToggleOption("umbrella", 100),
            new ToggleOption("daggers", 100),
            new ToggleOption("hammer", 100),
            new ToggleOption("sword_heavy", 150),
        };

        public static SelectionGridOption stats = new SelectionGridOption(new string[] { "nmg", "00" }, 100);

        public static ToggleOption[] fights;

        public static void Init()
        {
            var tempFights = new List<ToggleOption>();
            foreach (var i in FightStorage.defaultFightOrder)
            {
                tempFights.Add(new ToggleOption(i.ToString(), 200, FightStorage.speedrunPresetFights.Contains(i)));
            }
            fights = tempFights.ToArray();

            var w = new OptionCategory("Starting Weapons", weapons);
            var s = new OptionCategory("Stat Preset", new BaseOption[] { stats });
            var f = new OptionCategory("Fights", fights, gapBetweenThings: 1);
            var main = new OptionCategory("Boss Rush Options", subCategories: new OptionCategory[] { w, s, f }, gapBetweenThings: 2);
            gui = new GUIBox(new UnityEngine.Vector2(20, 20), main, 17);

            hasInit = true;
        }

        public static void OnGUI()
        {
            if (!hasInit)
            {
                Init();
            }
            gui.OnGUI();
        }
    }
}
