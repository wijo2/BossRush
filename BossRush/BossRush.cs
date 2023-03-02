using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BossRush
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class BossRush : BaseUnityPlugin
    {
        private const string pluginGuid = "ddoor.BossRush.wijo";
        private const string pluginName = "BossRush";
        private const string pluginVersion = "0.0.3";

        public static bool active = false; //is the mod active in current playing session

        public static String[] flags = new string[] { "tut_firstintro", "fomo_intro", "graveyard_westknight", "redeemer_cutscene_watched", "crowboss_intro_watched", "yetiboss_cutscene_watched", "grandma_fight_intro_seen", "grandma_boss_intro_watched", "frog_ghoul_intro", "frog_boss_intro_seen", "frogboss_cutscene_watched", "lod_gauntlet_intro1", "lod_gauntlet_intro2", "lod_gauntlet_intro3", "lod_gauntlet_intro4", "lod_gauntlet_intro5", "lod_gauntlet_intro_done", "lod_demon1", "lod_demon2", "lod_demon3", "lod_demon4", "finallod_intro" };

        public static List<FightData> fightsYetToBeFought = new List<FightData>();
        public static List<FightName> fightsInRun = new List<FightName>();

        public static int FightCounter = 0;
        public static bool canAddToFughtCounter = false; //needed bc onSceneLoaded apparently can happen multiple times idk what that's about

        public static bool ignoreNextFadeIn = false; //bodged attempt to fix some fades, idk if it even works but I'm too scared to touch it anymore

        public static bool startOnNextLoad = false;

        public static float titleTimer = 0; //timer for autoquit after killing lod
        public static bool quitOnNextFade = false;

        internal static ManualLogSource Log;

        public static GameSave save;

        public static string debug;

        public static GUIBox gui;

        public void Awake()
        {
            Log = base.Logger;

            var harmony = new Harmony(pluginGuid);
            harmony.PatchAll(typeof(BossRush));

            OptionsMenu.Init();
            gui = OptionsMenu.gui;
        }

        public void FixedUpdate()
        {
            if (titleTimer > 0)
            {
                titleTimer -= Time.fixedDeltaTime;
                if (titleTimer <= 0)
                {
                    SQ();
                }
            }

            if (quitOnNextFade == true && !ScreenFade.instance.IsFading()) { SQ(); }
        }

        public static void SQ()
        {
            PlayerGlobal.instance.UnPauseInput();
            UIMenuPauseController.instance.Pause();
            var o = Resources.FindObjectsOfTypeAll<UIMenuOptions>()[0];
            o.gameObject.SetActive(true);
            o.ExitSession();
            o.ClosePrompt_ExitToMenu(true);
        }

        public static void L(string text) { Log.LogWarning(text); } //debugging c:

        [HarmonyPatch(typeof(GameSceneManager), "OnSceneLoaded")]
        [HarmonyPostfix]
        public static void DoSceneStartThings()
        {
            save = GameSave.GetSaveData();
            if (!active || SceneManager.GetActiveScene().name == "TitleScreen") { return; }

            if (PlayerGlobal.instance && !PlayerGlobal.instance.DidJustRespawn() && canAddToFughtCounter)
            {
                FightCounter += 1;
                canAddToFughtCounter = false;
            }

            var ws = WeaponSwitcher.instance;
            ws.UnlockBombs();
            ws.UnlockFire();
            ws.UnlockHooskhot();
            GameSave.currentSave.Save();
            GameSave.currentSave.Load();

            if (OptionsMenu.stats.state == 0)
            {
                var l = FightStorage.GiveStats(fightsInRun[FightCounter]);
                Inventory.instance.SetItemCount("stat_melee", l[0]);
                Inventory.instance.SetItemCount("stat_dexterity", l[1]);
                Inventory.instance.SetItemCount("stat_haste", l[2]);
                Inventory.instance.SetItemCount("stat_magic", l[3]);
                Inventory.instance.AddItem("stat_melee", 0);
                Inventory.instance.AddItem("stat_dexterity", 0);
                Inventory.instance.AddItem("stat_haste", 0);
                Inventory.instance.AddItem("stat_magic", 0);
            }

            Inventory.instance.RemoveItem(Inventory.instance.GetItem("sword"));
            var exists = "";
            foreach (var i in OptionsMenu.weapons)
            {
                if (i.getState())
                {
                    if (exists == "")
                    {
                        exists = i.text;
                    }
                    Inventory.instance.SetItemCount(i.text, 1);
                }
            }
            PlayerEquipment.instance.SetRightHandWeapon(exists);

            //Inventory.instance.SetItemCount("stat_melee", 9999); //debug buffs
            //Time.timeScale = 6;

            if (PlayerGlobal.instance != null)
            {
                PlayerGlobal.instance.SetInvul(0); //this has an error sometimes idk why it doesn't make sense
                PlayerGlobal.instance.SetPosition(fightsYetToBeFought[0].position);
                PlayerGlobal.instance.UnPauseInput_Cutscene();
                PlayerGlobal.instance.gameObject.GetComponent<PlayerMovementControl>().SetCanLimitVelocity(true);
                PlayerGlobal.instance.gameObject.GetComponentInChildren<WeaponControl>().animationControl.EndSlash();
            }

            save.SetKeyState("c_oldcrowdead", false, false);

            save.SetSpawnPoint(SceneManager.GetActiveScene().name, null);

            if (new string[] { "AVARICE_WAVES_Mansion", "AVARICE_WAVES_Forest", "AVARICE_WAVES_Fortress" }.Contains(SceneManager.GetActiveScene().name))
            {
                var c = GameObject.Find("IntroCutscene").GetComponent<Cutscene>();
                c.Trigger();
                c.EndCutscene();
            }
        }

        [HarmonyPatch(typeof(DamageableCharacter), "handleNoHealth")]
        [HarmonyPatch(typeof(DamageableLordOfDoors), "handleNoHealth")]
        [HarmonyPrefix]
        public static bool ThingDead(DamageableCharacter __instance)
        {
            //L(__instance.gameObject.name + " died :c");
            if (!active || __instance.gameObject.name != fightsYetToBeFought[0].goal) { return true; }
            //L("It's a boss c:");

            switch (__instance.gameObject.name)
            {
                case "_FORESTMOTHER_BOSS":
                    LoadNextMap();
                    return false;
                case "redeemer_BOSS":
                    LoadNextMap();
                    ignoreNextFadeIn = true;
                    break;
                case "betty_boss":
                    LoadNextMap();
                    break;
                case "FROG_BOSS_FAT":
                    LoadNextMap();
                    break;
                case "grandma":
                    LoadNextMap();
                    return false;
                case "old_crow_boss_fbx":
                    LoadNextMap();
                    break;
                case "BOSS_lord_of_doors NEW":
                    titleTimer = 2;
                    break;
                default:
                    Log.LogWarning("Couldn't identify boss");
                    break;
            }
            return true;
        }

        public static void LoadNextMap(float fadeOutTime = 1f)
        {
            if (PlayerGlobal.instance)
            {
                PlayerGlobal.instance.SetInvul(999);
                PlayerGlobal.instance.gameObject.GetComponent<PlayerMovementControl>().SetCanLimitVelocity(true);
                PlayerGlobal.instance.gameObject.transform.parent.GetComponentInChildren<PlayerAnimation>().EndSlash();
            }

            if (fightsYetToBeFought.Count() > 1)
            {
                canAddToFughtCounter = true;
                fightsYetToBeFought.RemoveAt(0);
                if (fadeOutTime != 0)
                {
                    ScreenFade.instance.FadeOut(fadeOutTime, true, null);
                    ScreenFade.instance.LockFade();
                }
                GameSceneManager.LoadSceneFadeOut(fightsYetToBeFought[0].scene, 0.7f);
                if (PlayerGlobal.instance)
                {
                    PlayerGlobal.instance.PauseInput();
                }
            }
            else
            {
                ignoreNextFadeIn = false;
                quitOnNextFade = true;
                ScreenFade.instance.FadeOut(1.2f, true, null);
                ScreenFade.instance.LockFade();
            }
            
        }

        [HarmonyPatch(typeof(EnemyWave), "endWave")]
        [HarmonyPrefix]
        public static bool WaveEnd(EnemyWave __instance)
        {
            if (!active) { return true; }
            var waveController = __instance.gameObject.GetComponentInParent<EnemyWaveController>();
            var lastWave = ((EnemyWave[])AccessTools.Field(typeof(EnemyWaveController), "waveList").GetValue(waveController)).Last();
            debug = FightStorage.GeneratePath(__instance);
            if (FightStorage.GeneratePath(__instance) == fightsYetToBeFought[0].goal && lastWave == __instance)
            {
                LoadNextMap();
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(SoulEmerge), "StartCutscene")]
        [HarmonyPrefix]
        public static bool StopSoulCut() { if (!active) { return true; } ignoreNextFadeIn = true; return false; }

        [HarmonyPatch(typeof(ScreenFade), "FadeIn")]
        [HarmonyPrefix]
        public static bool FadeInDecimator5000() { if (ignoreNextFadeIn && !active) { ignoreNextFadeIn = false; return false; } return true; }

        [HarmonyPatch(typeof(SaveSlot), "LoadSave")]
        [HarmonyPrefix]
        public static void PotentialStart(SaveSlot __instance, GameSave ___saveFile)
        {
            if (__instance.saveId == "slot3")
            {
                startOnNextLoad = true;
                active = true;
                ___saveFile.SetKeyState("cts_bus", true, true);
            }
        }

        [HarmonyPatch(typeof(GameSceneManager), "LoadSceneFadeOut")]
        [HarmonyPrefix]
        public static bool DisableFirstLoad()
        {
            if (startOnNextLoad)
            {
                startOnNextLoad = false;

                //flags
                var c = GameSave.currentSave;
                var copy = c.boolKeys.Keys.ToArray();
                foreach (var flag in copy)
                {
                    c.boolKeys[flag] = false;
                }

                foreach (var name in flags)
                {
                    GameSave.currentSave.SetKeyState(name, true, true);
                }

                //fights list
                var newFights = new List<FightName>();
                foreach (var fight in OptionsMenu.fights)
                {
                    FightName k = (FightName)Enum.Parse(typeof(FightName), fight.text);
                    if (fight.getState())
                    {
                        newFights.Add(k);
                    }
                }
                fightsInRun = new List<FightName>(newFights);
                fightsYetToBeFought.Clear();
                fightsYetToBeFought.Add(new FightData());
                foreach (var fight in newFights)
                {
                    fightsYetToBeFought.Add(FightStorage.fightDatas[fight]);
                }
                FightCounter = -1;

                //start!
                LoadNextMap(0);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(PlayerGlobal), "SpawnInjuredFalling")]
        [HarmonyPrefix]
        public static bool StopInjuredFall() { if (!active) { return true; } return false; }

        [HarmonyPatch(typeof(TitleScreen), "Start")]
        [HarmonyPrefix]
        public static void EndActive() { active = false; quitOnNextFade = false; ignoreNextFadeIn = false; }

        [HarmonyPatch(typeof(Cutscene), "Trigger")]
        [HarmonyPrefix]
        public static bool stopLODCut()
        {
            if (!(SceneManager.GetActiveScene().name == "lvl_HallOfDoors_BOSSFIGHT") || !active) { return true; }
            PlayerGlobal.instance.PauseInput();
            UI_Control.HideUI();
            return false;
        }

        [HarmonyPatch(typeof(DeathText), "openRetryPrompt")]
        [HarmonyPrefix]
        public static bool instantRetry()
        {
            if (!active) { return true; }
            RespawnPrompter.instance.Retry();
            return false;
        }

        [HarmonyPatch(typeof(AvariceEnding), "End")]
        [HarmonyPrefix]
        public static bool AvaCutsceneDisable() { return !active; }

        public void OnGUI()
        {
            if (TitleScreen.instance && TitleScreen.instance.saveMenu.HasControl())
            {
                OptionsMenu.OnGUI();
            }
        }
    }
}
