using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossRush
{
    public static class OptionsMenu
    {
        public static GUIBox.GUIBox leftgui;
        public static GUIBox.GUIBox rightgui;
        public static bool hasInit = false;

        //style
        public static GUIBox.SelectionGridOption styleSelect = new GUIBox.SelectionGridOption(new string[] { "speedrun/practise", "rougelike" });

        //speedrun select
        public static GUIBox.ToggleOption[] sWeapons = new GUIBox.ToggleOption[]
        {
            new GUIBox.ToggleOption("sword", true),
            new GUIBox.ToggleOption("hammer"),
            new GUIBox.ToggleOption("daggers"),
            new GUIBox.ToggleOption("greatsword"),
            new GUIBox.ToggleOption("umbrella")
        };

        public static GUIBox.ToggleOption[] sUpgrades = new GUIBox.ToggleOption[]
        {
            new GUIBox.ToggleOption("arrows"),
            new GUIBox.ToggleOption("fire"),
            new GUIBox.ToggleOption("bombs"),
            new GUIBox.ToggleOption("hookshot"),
        };

        public static GUIBox.SelectionGridOption stats = new GUIBox.SelectionGridOption(new string[] { "00", "nmg", "nmgLow", "nmgLowHasty" });

        //rouglike select
        public static GUIBox.SelectionGridOption rWeaponStyle = new GUIBox.SelectionGridOption(new string[] { "Random Weapon", "Unlock Weapons (wip)" });
        public static GUIBox.SelectionGridOption rStartigWeapon = new GUIBox.SelectionGridOption(new string[]
        {
            "sword",
            "hammer",
            "daggers",
            "greatsword",
            "umbrella"
        });
        public static GUIBox.NumberFieldOption rHealCooldown = new GUIBox.NumberFieldOption(0.04f, "Heal 1 hp every x arenas", initialState: 2);

        //fight select
        public static GUIBox.ToggleOption[] fights;
        public static GUIBox.ButtonOption defAll;
        public static GUIBox.ButtonOption defNone;

        public static void Init()
        {
            defAll = new GUIBox.ButtonOption("Select All");
            defNone = new GUIBox.ButtonOption("Select None");

            var tempFights = new List<GUIBox.ToggleOption>();
            foreach (var i in FightStorage.defaultFightOrder)
            {
                tempFights.Add(new GUIBox.ToggleOption(i.ToString(), FightStorage.speedrunPresetFights.Contains(i)));
            }
            fights = tempFights.ToArray();


            var style = new GUIBox.OptionCategory("Mode", new GUIBox.BaseOption[] { styleSelect }, titleSizeMultiplier: 1.2f);

            //speedrun section
            var weapon = new GUIBox.OptionCategory("Weapons", sWeapons);
            var upgrades = new GUIBox.OptionCategory("Upgrades", sUpgrades);
            var stat = new GUIBox.OptionCategory("Stat Preset", new GUIBox.BaseOption[] { stats });
            var run = new GUIBox.OptionCategory("Speedrun Options", subCategories: new GUIBox.OptionCategory[] { weapon, upgrades, stat }, titleSizeMultiplier:1.7f);

            //roukelike section
            var wstyle = new GUIBox.OptionCategory("Weapon Style", new GUIBox.BaseOption[] {rWeaponStyle});
            var sweapon = new GUIBox.OptionCategory("Starting Weapon", new GUIBox.BaseOption[] { rStartigWeapon });
            var heal = new GUIBox.OptionCategory(options: new GUIBox.BaseOption[] { rHealCooldown });
            var rouge = new GUIBox.OptionCategory("Rougelike Options", subCategories: new GUIBox.OptionCategory[] {wstyle, sweapon, heal}, titleSizeMultiplier: 1.7f);

            //main
            var main = new GUIBox.OptionCategory("Boss Rush Options", subCategories: new GUIBox.OptionCategory[] { style, run, rouge });
            leftgui = new GUIBox.GUIBox(new UnityEngine.Vector2(0.01f, 0.01f), main);

            //fight select
            var quickSelect = new GUIBox.HorizontalOptionCategory(options: new GUIBox.BaseOption[] { defAll, defNone });
            var fight = new GUIBox.OptionCategory("Included Fights", fights);
            var fightMenu = new GUIBox.OptionCategory(subCategories: new GUIBox.OptionCategory[] { fight, quickSelect });
            rightgui = new GUIBox.GUIBox(new UnityEngine.Vector2(0.8f, 0.01f), fightMenu);

            hasInit = true;
        }

        public static void OnGUI()
        {
            if (!hasInit)
            {
                Init();
            }

            leftgui.contents.subCategories[1].SetActive(styleSelect.GetState() == 0);
            leftgui.contents.subCategories[2].SetActive(styleSelect.GetState() == 1);
            leftgui.contents.subCategories[2].subCategories[1].SetActive(rWeaponStyle.GetState() == 1);
            leftgui.OnGUI();
            rightgui.OnGUI();

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

        public static void SetWeapons()
        {
            Inventory.instance.RemoveItem(Inventory.instance.GetItem("sword"));
            if (styleSelect.GetState() == 0)
            {
                var exists = "";
                foreach (var i in OptionsMenu.sWeapons)
                {
                    if (i.GetState())
                    {
                        if (exists == "")
                        {
                            exists = GetWeaponName(i.text);
                        }
                        Inventory.instance.SetItemCount(GetWeaponName(i.text), 1);
                    }
                }
                PlayerEquipment.instance.SetRightHandWeapon(exists);
            }
            else
            {
                if (rWeaponStyle.GetState() == 0)
                {
                    var name = GetWeaponName(rStartigWeapon.texts[UnityEngine.Random.Range(0, 4)]);
                    Inventory.instance.SetItemCount(name, 1);
                    PlayerEquipment.instance.SetRightHandWeapon(name);
                }
                else
                {
                    var name = GetWeaponName(rStartigWeapon.texts[rStartigWeapon.GetState()]);
                    Inventory.instance.SetItemCount(name, 1);
                    PlayerEquipment.instance.SetRightHandWeapon(name);
                }
            }
        }

        public static void SetUpgrades()
        {
            GameSave.GetSaveData().SetKeyState("upgraded_arrows", sUpgrades[0].GetState());
            GameSave.GetSaveData().SetKeyState("upgraded_fire", sUpgrades[1].GetState());
            GameSave.GetSaveData().SetKeyState("upgraded_bombs", sUpgrades[2].GetState());
            GameSave.GetSaveData().SetKeyState("upgraded_hookshot", sUpgrades[3].GetState());
        }

        public static string GetWeaponName(string name)
        {
            switch (name)
            {
                case "sword":
                    return "sword";
                case "hammer":
                    return "hammer";
                case "daggers":
                    return "daggers";
                case "greatsword":
                    return "sword_heavy";
                case "umbrella":
                    return "umbrella";
                default:
                    return "";
            }
        }
    }
}
