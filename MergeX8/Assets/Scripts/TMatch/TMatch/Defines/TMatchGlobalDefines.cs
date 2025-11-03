using UnityEngine;

namespace TMatch
{


    public enum TMatchDifficulty
    {
        Normal = 1, //普通
        Hard = 2, //困难
        Demon = 3, //恶魔
    }

// 奖励展示数据结构
    public struct TMatchRewardItemData
    {
        public string atlasName;
        public string iconName;
        public bool isShowNum;
        public int num;
        public Sprite sprite;

        public TMatchRewardItemData(string inAtlasName, string inIconName, bool inIsShowNum, int inNum = 0,Sprite inSprite = null)
        {
            atlasName = inAtlasName;
            iconName = inIconName;
            isShowNum = inIsShowNum;
            num = inNum;
            sprite = inSprite;
        }
    }

// 图集名字定义
    public class TMatchAtlasName
    {
        public const string Boost = "BoostAtlas";
        public const string Common01 = "Common01Atlas";
    }
    
    /// <summary>
    /// 复活 关联系统
    /// </summary>
    public enum TMatchReviveSystem
    {
        NoSystem = 0,   // 无
        ReviveGiftPack, // 复活礼包
        GoldenPass      // 战令
    }
}