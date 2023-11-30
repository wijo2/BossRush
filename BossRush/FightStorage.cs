using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossRush
{
    public static class FightStorage
    {
        public static Dictionary<FightName, FightData> fightDatas = new Dictionary<FightName, FightData>() //{name, fightData entry}
        {
            //bosses

            {FightName.dfs, new FightData("lvl_Tutorial", new Vector3(-1f, 1f, -11.5f), "_FORESTMOTHER_BOSS") },
            {FightName.gotd, new FightData("lvl_Graveyard", new Vector3(155f, 44f, 340.5f), "redeemer_BOSS") },
            {FightName.betty, new FightData("boss_betty", new Vector3(96f, 96f, 106f), "betty_boss") },
            {FightName.frog, new FightData("boss_Frog", new Vector3(-273.5f, 86.5f, 1176f), "FROG_BOSS_FAT") },
            {FightName.grandma, new FightData("boss_Grandma", new Vector3(-1062f, -32f, 909f), "grandma") },
            {FightName.gc, new FightData("OldCrowVoid", new Vector3(0f, 1f, 0f), "old_crow_boss_fbx") },
            {FightName.lod, new FightData("lvl_HallOfDoors_BOSSFIGHT", new Vector3(-556f, 499f, -45.5f), "BOSS_lord_of_doors NEW") },
            {FightName.steadhone, new FightData("lvl_Graveyard", new Vector3(21.9f, 56.64f, 411.6f), "BOSS_GraveDigger") },

            //arenas

            // graveyard
            {FightName.graveyard_1, new FightData("lvl_Graveyard", new Vector3(87.43f, 46.00f, 288.09f), "MainRoom/_CONTENTS/LevelChunks/18/_CONTENTS/EmptyWaveBattle/WaveControl") },
            {FightName.graveyard_2, new FightData("lvl_Graveyard", new Vector3(210.00f, 37.00f, 195.00f), "MainRoom/_CONTENTS/EmptyWaveBattle (1)/WaveControl") },

            // mansion
            /*{FightName.manor_crow_1, new FightData("lvl_GrandmaMansion", new Vector3(20.00f, 38.00f, 1158.00f), "_SCENE_MOVER/GroundFloor/SecretMagic/R_LeftSecret_Fight/_CONTENTS/GrimaceFightSelector/WaveControl1") },
            {FightName.manor_crow_3, new FightData("lvl_GrandmaMansion", new Vector3(-267.00f, 38.00f, 1084.00f), "_SCENE_MOVER/GroundFloor/SecretBombs/R_BedroomSecret_Fight/_CONTENTS/GrimaceFightSelector/WaveControl2") },
            {FightName.manor_crow_4, new FightData("lvl_GrandmaMansion", new Vector3(-150.00f, 49.00f, 1224.00f), "_SCENE_MOVER/R_LibrarySecret_Fight/_CONTENTS/GrimaceFightSelector/WaveControl3") },*/

            // forest
            {FightName.forest_roman_gardens, new FightData("lvl_Forest", new Vector3(575.00f, -69.00f, 612.00f), "_SceneMover/Room_ForestMain/_CONTENTS/ForestMain/RomanGardens/Waves1/WaveControl") },
            {FightName.forest_mushroom, new FightData("lvl_Forest", new Vector3(500.00f, -44.00f, 797.00f), "_SceneMover/Room_ForestMain/_CONTENTS/ForestMain/Mushroom/Waves2/WaveControl") },
            {FightName.dungeon_crow_1, new FightData("lvl_Forest", new Vector3(511.00f, -101.00f, 769.00f), "_SceneMover/Dungeon/Room_WaveBattle/_CONTENTS/EmptyWaveBattle_Empty (2)/WaveControl") },
            {FightName.dungeon_crow_2, new FightData("lvl_Forest", new Vector3(570.00f, -107.00f, 600.00f), "_SceneMover/Dungeon/Room_EastExit3/_CONTENTS/EmptyWaveBattle_Empty (1)/WaveControl") },
            {FightName.dungeon_crow_3, new FightData("lvl_Forest", new Vector3(522.50f, -117.00f, 472.00f), "_SceneMover/Dungeon/ROOM_EnterLadder (1)/_CONTENTS/EmptyWaveBattle_Empty/WaveControl") },
            {FightName.dungeon_water, new FightData("lvl_Forest", new Vector3(376.50f, -104.50f, 592.50f), "_SceneMover/Dungeon/Room_Corridor2/_CONTENTS/EmptyWaveBattle_Empty/WaveControl") },

            // swamp
            {FightName.ff_big, new FightData("lvl_Swamp", new Vector3(877.00f, -111.00f, 122.50f), "_SceneMover/EmptyWaveBattle/WaveControl") },

            // lockstone
            {FightName.lockstone_crow_1, new FightData("lvl_FrozenFortress", new Vector3(-650f, -0.90f, 73.50f), "SceneMover/Hookshot_Dungeon/Ground_Floor/SouthWest (1)/_CONTENTS/EmptyWaveBattle_Empty/WaveControl") },
            {FightName.lockstone_crow_2, new FightData("lvl_FrozenFortress", new Vector3(-843.00f, 28.75f, 121.6f), "SceneMover/Hookshot_Dungeon/Upper_Floor/North_Upper_COLLIDERCHECK/_CONTENTS/EmptyWaveBattle_Empty (1)/WaveControl") },
            {FightName.lockstone_crow_3, new FightData("lvl_FrozenFortress", new Vector3(-850.00f, 4.00f, 276.00f), "SceneMover/Hookshot_Dungeon/Ground_Floor/R_Colliseum/_CONTENTS/EmptyWaveBattle_Empty (2)/WaveControl") },
            {FightName.lockstone_crow_4, new FightData("lvl_FrozenFortress", new Vector3(-845.00f, -2.00f, 20.00f), "SceneMover/Hookshot_Dungeon/Ground_Floor/NorthWest/_CONTENTS/EmptyWaveBattle_Empty/WaveControl") },

            // watchtowers
            {FightName.ice_arena, new FightData("lvl_mountaintops", new Vector3(-550.00f, 140.50f, 775.00f), "R_Mountaintops/_CONTENTS/WaveBattleArena/EmptyWaveBattle_Empty/WaveControl") },

            // avas
            {FightName.ava1, new FightData("AVARICE_WAVES_Mansion", new Vector3(0, 1, 5), "Room_Main/_CONTENTS/EmptyWaveBattle/WaveControl") },
            {FightName.ava2, new FightData("AVARICE_WAVES_Forest", new Vector3(0, 1, 12.24f), "Room_Main/_CONTENTS/EmptyWaveBattle/WaveControl") },
            {FightName.ava3, new FightData("AVARICE_WAVES_Fortress", new Vector3(0, 1, 5), "Room_Main/_CONTENTS/EmptyWaveBattle/WaveControl") },
        };

        public static FightName[] defaultFightOrder = new FightName[] 
        { 
            FightName.dfs, 
            FightName.graveyard_1, 
            FightName.graveyard_2, 
            FightName.gotd, 
            /*FightName.manor_crow_1, these need special work cause always same order thing not worth for now at least
            FightName.manor_crow_3,
            FightName.manor_crow_4,*/
            FightName.ava1,
            FightName.forest_roman_gardens, 
            FightName.forest_mushroom,
            FightName.dungeon_crow_1,
            FightName.dungeon_crow_2,
            FightName.dungeon_crow_3,
            FightName.dungeon_water,
            FightName.ava2,
            FightName.lockstone_crow_1,
            FightName.lockstone_crow_2,
            FightName.lockstone_crow_3,
            FightName.lockstone_crow_4,
            FightName.ava3,
            FightName.ice_arena,
            FightName.betty,
            FightName.ff_big,
            FightName.frog,
            FightName.grandma,
            FightName.gc,
            FightName.lod,
            FightName.steadhone
        };

        public static FightName[] speedrunPresetFights = new FightName[]
        {
            FightName.dfs,
            FightName.graveyard_1,
            FightName.gotd, 
            FightName.ava1,
            FightName.forest_mushroom,
            FightName.dungeon_water,
            FightName.ava2,
            FightName.lockstone_crow_1,
            FightName.ava3,
            FightName.ice_arena,
            FightName.betty,
            FightName.ff_big,
            FightName.frog,
            FightName.grandma,
            FightName.gc,
            FightName.lod
        };

        public static Dictionary<int, int[]> zeroStats = new Dictionary<int, int[]>() 
        {
        };

        public static Dictionary<int, int[]> nmgStatPreset = new Dictionary<int, int[]>() //index = fight where you first have stats (starts at 0 cause list)
        {
            {4, new int[] {2,0,0,0} },
            {8, new int[] {3,0,0,0} },
            {12, new int[] {4,0,0,0} },
            {21, new int[] {5,1,0,0} },
            {23, new int[] {5,2,0,0} }
        };

        public static Dictionary<int, int[]> nmgLowStatPreset = new Dictionary<int, int[]>() //index = fight where you first have stats (starts at 0 cause list)
        {
            {5, new int[] {2,0,0,0} },
            {12, new int[] {3,0,0,0} },
            {19, new int[] {4,0,0,0} },
            {23, new int[] {5,0,0,0} },
        };

        public static Dictionary<int, int[]>[] statDicts = new Dictionary<int, int[]>[] { zeroStats, nmgStatPreset, nmgLowStatPreset };

        public static int[] GiveStats(FightName fight, Dictionary<int, int[]> statDict)
        {
            var c = Array.FindIndex(defaultFightOrder, element => element == fight);

            for (var i = c; i >= 0; i--)
            {
                if (statDict.ContainsKey(i))
                {
                    return statDict[i];
                }
            }
            return new int[] { 0, 0, 0, 0 };
        }

        public static string GeneratePath(EnemyWave instance)
        {
            var obj = instance.gameObject.transform.parent.gameObject;
            var path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }
    }

    public struct FightData
    {
        public string scene; //Scene the fight takes place in
        public Vector3 position; //Position to spawn at
        public string goal; //String key to keep track of ending fight

        public FightData(string scene, Vector3 position, string goal)
        {
            this.scene = scene;
            this.position = position;
            this.goal = goal;
        }
    }

    public enum FightName
    {
        dfs,
        gotd,
        betty,
        frog,
        grandma,
        gc,
        lod,
        steadhone,
        graveyard_1,
        graveyard_2,
        manor_crow_1,
        manor_crow_3,
        manor_crow_4,
        forest_roman_gardens,
        forest_mushroom,
        dungeon_crow_1,
        dungeon_crow_2,
        dungeon_crow_3,
        dungeon_water,
        ff_big,
        lockstone_crow_1,
        lockstone_crow_2,
        lockstone_crow_3,
        lockstone_crow_4,
        ice_arena,
        ava1,
        ava2,
        ava3
    }
}
