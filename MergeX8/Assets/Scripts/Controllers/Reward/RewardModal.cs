using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Game.Config;
using Gameplay;
using UnityEngine;

public class RewardModal : Manager<RewardModal>
{
    /// <summary>
    /// 获取宝箱奖励
    /// </summary>
    /// <param name="chestID"></param>
    /// <returns></returns>
    public List<ResData> GetLevelChestReward(int chestID)
    {
        //宝箱配置
        // TableLevelChest chest=  M3ConfigManager.Instance.GetLevelChestByID(chestID);
        // if (chest == null)
        //     return null;
        //
        // return ParseReward(chest.bonusList);
        return null;
    }

    /// <summary>
    /// 牌面宝箱
    /// </summary>
    /// <param name="chestID"></param>
    /// <returns></returns>
    public List<ResData> GetStageChestReward(int chestID)
    {
        // //宝箱配置
        // TableStageChest chest=  M3ConfigManager.Instance.GetStageChestByID(chestID);
        // if (chest == null)
        //     return null;
        // List<ResData> resDatas = new List<ResData>();
        // if (chest.tileList != null && chest.tileList.Length > 0)
        // {
        //     foreach (var item in chest.tileList)
        //     {
        //         resDatas.Add(new ResData(UserData.ResourceId.Tile,item));
        //     }   
        // }
        //
        // if (chest.themeList != null && chest.themeList.Length > 0)
        // {
        //     foreach (var item in chest.themeList)
        //     {
        //         resDatas.Add(new ResData(UserData.ResourceId.BackGround, item));
        //     }
        // }
        //
        // return resDatas;
        return null;
    }

    public List<ResData> ParseReward(string StrReward)
    {
        if (StrReward == null || StrReward == "")
            return null;
        List<ResData> resDatas = new List<ResData>();
        string[] items = StrReward.Split('#');
        if (items != null && items.Length > 0)
        {
            for (int i = 0; i < items.Length; i++)
            {
                string strItem = items[i];
                string[] rewardInfo = strItem.Split(',');

                resDatas.Add(new ResData(int.Parse(rewardInfo[0]), int.Parse(rewardInfo[1]))); //TODO:临时,奖励数量取下限
            }
        }

        return resDatas;
    }
}