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

        public static BaseOption[] optionReferences;

        public static ToggleOption[] weapons = new ToggleOption[]
        {
            new ToggleOption("sword", true),
            new ToggleOption("umbrella"),
            new ToggleOption("daggers"),
            new ToggleOption("hammer"),
            new ToggleOption("sword_heavy"),
        };

        public static SelectionGridOption stats = new SelectionGridOption(new string[] { "nmg", "00" }, 1.5f);

        public static ToggleOption[] fights;

        public static ButtonOption defAll;
        public static ButtonOption defNone;

        public static void Init()
        {
            defAll = new ButtonOption("Select All", 2);
            defNone = new ButtonOption("Select None", 2);

            var tempFights = new List<ToggleOption>();
            foreach (var i in FightStorage.defaultFightOrder)
            {
                tempFights.Add(new ToggleOption(i.ToString(), FightStorage.speedrunPresetFights.Contains(i)));
            }
            fights = tempFights.ToArray();

            var all = new ButtonOption("Enable All", 1.5f);

            var w = new OptionCategory("Starting Weapons", weapons, titleSizeMultiplier:1.5f);
            var s = new OptionCategory("Stat Preset", new BaseOption[] { stats }, titleSizeMultiplier: 1.5f);
            var f = new OptionCategory("Fights", fights, gapBetweenThings: 1, titleSizeMultiplier: 1.5f);
            var p = new HorizontalOptionCategory(options: new BaseOption[] { defAll, defNone });
            var main = new OptionCategory("Boss Rush Options", subCategories: new OptionCategory[] { w, s, f, p }, gapBetweenThings: 1, fontSize: 15);
            gui = new GUIBox(new UnityEngine.Vector2(20, 20), main, 10);

            hasInit = true;
        }

        public static void OnGUI()
        {
            if (!hasInit)
            {
                Init();
            }
            gui.OnGUI();

            if (defAll.IsPressed())
            {
                foreach (var f in fights)
                {
                    f.state = true;
                }
            }
            if (defNone.IsPressed())
            {
                foreach (var f in fights)
                {
                    f.state = false;
                }
            }
        }
    }
}
