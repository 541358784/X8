using System;
using System.Collections.Generic;
using System.Linq;
using Activity.BalloonRacing.Dynamic;
using Activity.GardenTreasure.View;
using Activity.JumpGrid;
using Activity.LuckyGoldenEgg;
using Activity.Matreshkas.View;
using Activity.RabbitRacing.Dynamic;
using Activity.SaveTheWhales;
using TotalRecharge_New;
using UnityEngine;

public partial class MergeTaskTipsController
{
    private Dictionary<string, List<Transform>> _siblingMap = new Dictionary<string, List<Transform>>();

    private string[] _dynamicSiblingSort = new String[]
    {
        typeof(MergeRVItem).ToString(),
        typeof(MergeDailyTaskItem).ToString(),
        typeof(MergeFarmBtn).ToString(),
        typeof(MergeRewardItem).ToString(),
        typeof(MergeExtraOrderRewardCoupon).ToString(),
        typeof(MergeCardPackage).ToString(),
        typeof(MergeBuyDiamondTicket).ToString(),
        typeof(MergeBattlePass2).ToString(),
        typeof(MergeBattlePass).ToString(),
        
        typeof(MergeKapiScrew).ToString(),
        typeof(MergeKapiTile).ToString(),
        typeof(MergeKapibala).ToString(),
        
        typeof(MergeZuma).ToString(),
        typeof(TotalRechargeDecoReward).ToString(),
        typeof(MergeTotalRecharge_New).ToString(),
        typeof(MergeTotalRecharge).ToString(),
        typeof(MergeMixMaster).ToString(),
        typeof(MergePillowWheel).ToString(),
        typeof(MergeCatchFish).ToString(),
        typeof(Activity.TrainOrder.Merge_TrainOrder).ToString(),
        typeof(MergeGarageCleanUp).ToString(),
        typeof(MergeSummerWatermelonBread).ToString(),
        typeof(MergeSummerWatermelon).ToString(),
        
        typeof(MergeTurntableEntry).ToString(),
        typeof(MergeSlotMachine).ToString(),
        typeof(MergeCoinRush).ToString(),
        typeof(MergeBiuBiu).ToString(),
        typeof(MergeButterflyWorkShop).ToString(),
        
        typeof(MergeStarrySkyCompass).ToString(),
        typeof(MergeTurtlePang).ToString(),
        
        
        typeof(MergeClimbTree).ToString(),
        typeof(MergeDogHope).ToString(),
        typeof(MergeParrot).ToString(),
        typeof(MergeFlowerField).ToString(),
        
        typeof(MergeJungleAdventure).ToString(),
        typeof(MergeFishCulture).ToString(),
        typeof(MergeGardenTreasureEntry).ToString(),
        typeof(MergeLuckyGoldenEgg).ToString(),
        typeof(MergeTreasureHunt).ToString(),
        
        typeof(MergeCoinCompetition).ToString(),
        typeof(MergeTaskItemJumpGrid).ToString(),
        typeof(MergeSeaRacing).ToString(),
        
        typeof(MergeCardCollection).ToString(),
        typeof(MergePigBoxInTaskController).ToString(),
        typeof(MergeGiftBagProgress).ToString(),
        
        typeof(MergeMermaid).ToString(),
        typeof(MergeThemeDecoration).ToString(),
        typeof(MergeThemeDecorationLeaderBoard).ToString(),
        typeof(MergeMonopoly).ToString(),
        typeof(MergeSnakeLadder).ToString(),
        typeof(MergeEaster2024).ToString(),
        typeof(MergePhotoAlbum).ToString(),
        typeof(MergeHappyGo).ToString(),
        
        typeof(MergeRecoverCoin).ToString(),
        typeof(MergeCoinLeaderBoard).ToString(),
        
        typeof(MergeKeepPet).ToString(),
        
        typeof(MergeMatreshkasEntry).ToString(),
        typeof(Game_BalloonRacing).ToString(),
        typeof(Game_RabbitRacing).ToString(),
        typeof(MergeSaveTheWhales).ToString(),
        // typeof(MergeTaskUnlockTipController).ToString(),
    };
    
    void RegisterSibling()
    {
        // AddSibling(typeof(MergeTaskUnlockTipController).ToString(), _mergeTaskUnlock.transform);
        AddSibling(typeof(MergeRVItem).ToString(), _mergeRVItem.transform);
        AddSibling(typeof(MergeDailyTaskItem).ToString(), _mergeDailyTaskItem.transform);
        AddSibling(typeof(MergeFarmBtn).ToString(), _mergeFarmBtn.transform);
        AddSibling(typeof(MergeRewardItem).ToString(), _mergeRewardItem.transform);
        AddSibling(typeof(MergeGarageCleanUp).ToString(), _mergeGarageCleanUp.transform);
        AddSibling(typeof(MergeRecoverCoin).ToString(), _mergeRecoverCoin.transform);
        AddSibling(typeof(MergeCoinLeaderBoard).ToString(), _mergeCoinLeaderBoard.transform);
        AddSibling(typeof(MergeSummerWatermelon).ToString(), _mergeSummerWatermelon.transform);
        AddSibling(typeof(MergeButterflyWorkShop).ToString(), _MergeButterflyWorkShop.transform);
        AddSibling(typeof(MergeTreasureHunt).ToString(), _MergeTreasureHunt.transform);
        AddSibling(typeof(MergeHappyGo).ToString(), _mergeHappyGo.transform);
        AddSibling(typeof(MergeEaster).ToString(), _mergeEaster.transform);
        AddSibling(typeof(MergeMermaid).ToString(), _MergeMermaid.transform);
        AddSibling(typeof(MergeTotalRecharge).ToString(), _MergeTotalRecharge.transform);
        AddSibling(typeof(MergeTotalRecharge_New).ToString(), _MergeTotalRecharge_New.transform);
        AddSibling(typeof(TotalRechargeDecoReward).ToString(), _totalRechargeNewDecoReward.transform);
        AddSibling(typeof(MergeExtraOrderRewardCouponShowView).ToString(), MergeExtraOrderRewardCouponShowView.transform);
        
    }
    
    private void AddSibling(string type, Transform trans)
    {
        if (!_siblingMap.ContainsKey(type))
            _siblingMap[type] = new List<Transform>();
        
        if(_siblingMap[type].Contains(trans))
            return;
        
        _siblingMap[type].Add(trans);

        UpdateSibling();
    }
    
    void UpdateSibling()
    {
        int index = 0;
        foreach (var type in _dynamicSiblingSort)
        {
            if(!_siblingMap.ContainsKey(type))
                continue;
                
            foreach (var trans in _siblingMap[type])
            {
                trans.SetSiblingIndex(index);
                index++;
            }
        }
        
        for (int i = 0; i < m_itemList.Count; i++)
        {
            m_itemList[i].SiblingIndex = i;
            m_itemList[i].transform.SetSiblingIndex(index);
            index++;
        }
        
        _mergeTaskUnlock?.transform.SetSiblingIndex(index);
    }
}