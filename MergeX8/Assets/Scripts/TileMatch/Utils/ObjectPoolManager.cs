using System.Collections.Generic;

namespace GamePool
{
    public partial class ObjectPoolManager
    {
        public void InitPool()
        {
            foreach (var kv in ObjectPoolName.PoolConfig)
            {
                CreatePool(kv.Key, kv.Value, ObjectPoolDelegate.CreateGameItem);
            }
        }

        public void DestroyPool()
        {
            foreach (var kv in ObjectPoolName.PoolConfig)
            {
                DestroyPool(kv.Key);
            }
        }
    }

    public partial class ObjectPoolName
    {
        public static string CommonFlyItem = "TileMatch/Prefabs/FlyItem";
        public static string TileMatchBlock = "TileMatch/Prefabs/block";
        public static string TileMatchBlock_Frog = "TileMatch/Prefabs/block_frog";
        public static string TileMatchBlock_Rope = "TileMatch/Prefabs/block_rope";
        public static string TileMatchBlock_Leaves = "TileMatch/Prefabs/block_leaves";
        public static string TileMatchBlock_Ice = "TileMatch/Prefabs/block_ice";
        public static string TileMatchBlock_Gold = "TileMatch/Prefabs/block_goldpaper";
        public static string TileMatchBlock_Curtain = "TileMatch/Prefabs/block_curtain";
        public static string TileMatchBlock_Glue = "TileMatch/Prefabs/block_glue";
        public static string TileMatchBlock_Glue_Break = "TileMatch/Prefabs/block_glue_break";
        public static string TileMatchBlock_Glue_BreakEffect = "TileMatch/Prefabs/block_glue_breakeffect";
        public static string TileMatchBlock_Glue_Funnel = "TileMatch/Prefabs/block_funnel";
        public static string TileMatchBlock_Blast = "TileMatch/Prefabs/Blast";
        public static string TileMatchBlock_Bomb_Break = "TileMatch/Prefabs/block_bomb_blocker";
        public static string TileMatchBlock_Bag= "TileMatch/Prefabs/block_bag";
        public static string TileMatchBlock_AnimationMagnet= "TileMatch/Prefabs/AnimationMagnet";
        public static string TileMatchBlock_AnimationPool= "TileMatch/Prefabs/AnimationPool";
        public static string TileMatchBlock_GoldpaperFX= "TileMatch/Prefabs/block_goldpaperFX";
        public static string TileMatchBlock_Plane= "TileMatch/Prefabs/block_plane";
        public static string TileMatchBlock_PurdahRoot_1= "TileMatch/Prefabs/block_Hangtag2_3";
        public static string TileMatchBlock_PurdahRoot_2= "TileMatch/Prefabs/block_Hangtag4_5";
        public static string TileMatchBlock_PurdahRoot_3= "TileMatch/Prefabs/block_Hangtag6_7";
        public static string TileMatchBlock_PurdahFly= "TileMatch/Prefabs/block_HangtagFly";
        public static string TileMatchBlock_PurdahFx= "TileMatch/Prefabs/block_HangtagFX";
        public static string TileMatchBlock_PurdahFxBroken= "TileMatch/Prefabs/block_HangtagFX1_1";
        public static Dictionary<string, int> PoolConfig = new Dictionary<string, int>()
        {
            {CommonFlyItem, 10},
            { TileMatchBlock, 30 },
            { TileMatchBlock_Frog, 4 },
            { TileMatchBlock_Rope, 3 },
            { TileMatchBlock_Leaves, 3 },
            { TileMatchBlock_Ice, 3 },
            { TileMatchBlock_Gold, 3 },
            { TileMatchBlock_Curtain, 3 },
            { TileMatchBlock_Glue, 3 },
            { TileMatchBlock_Glue_Break, 3 },
            { TileMatchBlock_Glue_BreakEffect, 4 },
            { TileMatchBlock_Glue_Funnel, 3 },
            { TileMatchBlock_Blast, 3 },
            { TileMatchBlock_Bomb_Break, 3 },
            { TileMatchBlock_Bag, 3 },
            { TileMatchBlock_AnimationMagnet, 1},
            { TileMatchBlock_AnimationPool, 1},
            { TileMatchBlock_GoldpaperFX, 3},
            { TileMatchBlock_PurdahRoot_1, 2},
            { TileMatchBlock_PurdahRoot_2, 2},
            { TileMatchBlock_PurdahRoot_3, 2},
            { TileMatchBlock_Plane, 3},
            { TileMatchBlock_PurdahFly, 6},
            { TileMatchBlock_PurdahFx, 6},
            { TileMatchBlock_PurdahFxBroken, 6},
        };
    }
}