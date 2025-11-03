using UnityEngine;

namespace TileMatch.Game
{
    public enum BlockTypeEnum
    {
        Normal = 0,  //默认
        Glue, //黏胶
        Ice,   //冰块
        Lock,   //锁
        Funnel,   //漏斗
        Grass,   //草丛
        Frog,   //青蛙
        Bomb,   //炸弹
        Curtain,   //帘子
        Wrapper,   //包装纸
        Gold, //黄金牌
        Purdah,   //帷幕
        Plane,  //飞机
    }
    
    public enum NeighborEnum
    {
        None=-1,
        Up = 0,
        Down,
        Left,
        Right,
        Count,
    }
    public class GameConst
    {
        public static int COLLECT_MAX_LENGHT = 7;
        public static int COLLECT_EXTEND_MAX_LENGHT = 8;
        
        public static int BlockWidth = 136;
        public static int BlockHeight = 127;
        public static int BlockImageHeight = 152;

        public static int NOVAILDID = 65534;

        public const string TimeKey = "Time";
        public const string IceKey = "Ice";
        public const string BombKey = "Bomb";
        public static int IceBrokenMaxNum = 4;
        
        public static Vector2 GlobalOffset = new Vector2(0f, 2.25f);

        public static float Banner_StartX = -4.08f;
        public static float Banner_OffsetX = 1.36f;
        public static int Banner_MaxLength = 15;

        public const int SuperBackMaxLength = 3;
        public static float Super_Banner_OffsetY = 2.1f;

        public static float InitCollectBannerScale = 1f;
        public static float ExtendCollectBannerScale = 0.92f;
        
        public static Vector2 InitCollectBannerSize = new Vector2(9.8f, 1.8f);
        public static Vector2 ExtendCollectBannerSize = new Vector2(10.3f, 1.65f);
        public static float Banner_Extend_StartX = -4.39f;

        public static float Block_Gray_Alpha = 0.55f;
        public static float Block_Dark_Alpha = 0.7f;


        public static int BlockMinId = 1;
        public static int BlockMaxId = 34;

        public static int MaxLevelId = 600;
    }
}