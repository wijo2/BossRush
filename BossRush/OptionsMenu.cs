using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossRush
{
    public static class OptionsMenu
    {
        public static GUIBox.GUIBox gui;
        public static bool hasInit = false;

        public static GUIBox.SelectionGridOption styleSelect = new GUIBox.SelectionGridOption(new string[] { "speedrun/practise", "rougelike" });

        public static GUIBox.ToggleOption[] weapons = new GUIBox.ToggleOption[]
        {
            new GUIBox.ToggleOption("sword", true),
            new GUIBox.ToggleOption("umbrella"),
            new GUIBox.ToggleOption("daggers"),
            new GUIBox.ToggleOption("hammer"),
            new GUIBox.ToggleOption("sword_heavy"),
        };

        public static GUIBox.SelectionGridOption stats = new GUIBox.SelectionGridOption(new string[] { "00", "nmg", "nmgLow" }, 1.5f);

        public static GUIBox.ToggleOption[] fights;

        public static GUIBox.ButtonOption defAll;
        public static GUIBox.ButtonOption defNone;

        public static void Init()
        {
            defAll = new GUIBox.ButtonOption("Select All", 2);
            defNone = new GUIBox.ButtonOption("Select None", 2);

            var tempFights = new List<GUIBox.ToggleOption>();
            foreach (var i in FightStorage.defaultFightOrder)
            {
                tempFights.Add(new GUIBox.ToggleOption(i.ToString(), FightStorage.speedrunPresetFights.Contains(i)));
            }
            fights = tempFights.ToArray();


            var style = new GUIBox.OptionCategory("Mode", new GUIBox.BaseOption[] { styleSelect }, titleSizeMultiplier: 1.2f);

            //speedrun section
            var weapon = new GUIBox.OptionCategory("Starting Weapons", weapons, titleSizeMultiplier:1.5f);
            var stat = new GUIBox.OptionCategory("Stat Preset", new GUIBox.BaseOption[] { stats }, titleSizeMultiplier: 1.5f);
            var fight = new GUIBox.OptionCategory("Fights", fights, gapBetweenThings: 1, titleSizeMultiplier: 1.5f);
            var quickSelect = new GUIBox.HorizontalOptionCategory(options: new GUIBox.BaseOption[] { defAll, defNone });
            var run = new GUIBox.OptionCategory("Speedrun Options", subCategories: new GUIBox.OptionCategory[] { weapon, stat, fight, quickSelect }, gapBetweenThings: 1, fontSize: 15, titleSizeMultiplier:1.7f);


            var main = new GUIBox.OptionCategory("Boss Rush Options", subCategories: new GUIBox.OptionCategory[] { style, run });
            gui = new GUIBox.GUIBox(new UnityEngine.Vector2(20, 20), main, 10);

            hasInit = true;
        }

        public static void OnGUI()
        {
            if (!hasInit)
            {
                Init();
            }
            gui.contents.subCategories[1].SetActive(styleSelect.GetState() == 0);
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
