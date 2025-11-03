using System;
using System.Collections.Generic;
using System.ComponentModel;
using Activity.CrazeOrder.Model;
using Activity.GardenTreasure.Model;
using Activity.LimitTimeOrder;
using Activity.TimeOrder;
using Activity.Turntable.Model;
using DragonPlus;
using DragonPlus.Config.CrazeOrder;
using DragonPlus.Config.LimitOrderLine;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;


public partial class SROptions
{
    private const string GardenTreasure = "5花园宝藏";
    [Category(GardenTreasure)]
    [DisplayName("重制花园宝藏")]
    public void ClearGardenTreasure()
    {
        HideDebugPanel();
        GardenTreasureModel.Instance.GardenTreasure.Clear();

        int shovel = UserData.Instance.GetRes(UserData.ResourceId.GardenShovel);
        UserData.Instance.ConsumeRes(UserData.ResourceId.GardenShovel, shovel, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        
        int bomb = UserData.Instance.GetRes(UserData.ResourceId.GardenBomb);
        UserData.Instance.ConsumeRes(UserData.ResourceId.GardenBomb, bomb, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        
        var guideIdList = new List<int>() {4201,4202};
        CleanGuideList(guideIdList);
    }
    
    
    [Category(GardenTreasure)]
    [DisplayName("重制当前关卡")]
    public void ClearCurrentLevel()
    {
        HideDebugPanel();
        GardenTreasureModel.Instance.GardenTreasure.GetShapes.Clear();
        GardenTreasureModel.Instance.GardenTreasure.OpenGrids.Clear();
    }
    
    [Category(GardenTreasure)]
    [DisplayName("铲子")]
    public int GardenShovel
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.GardenShovel); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            
            if (value > 0)
            {
                UserData.Instance.SetRes(UserData.ResourceId.GardenShovel, value, reason);
            }
            else
            {
                UserData.Instance.ConsumeRes(UserData.ResourceId.GardenShovel, Math.Abs(value), reason);
            }
        }
    }
    
    
    [Category(GardenTreasure)]
    [DisplayName("普通关卡id")]
    public int NormalLevelId
    {
        get { return GardenTreasureModel.Instance.GardenTreasure.NormalLevelId; }
        set
        {
            GardenTreasureModel.Instance.GardenTreasure.NormalLevelId = value;
        }
    }
    
    
    [Category(GardenTreasure)]
    [DisplayName("随机关卡id")]
    public int RandomLevelId
    {
        get { return GardenTreasureModel.Instance.GardenTreasure.RandomLevelId; }
        set
        {
            GardenTreasureModel.Instance.GardenTreasure.RandomLevelId = value;
        }
    }
    
    
    [Category(GardenTreasure)]
    [DisplayName("是否进入随机关卡")]
    public bool IsRandomLevel
    {
        get { return GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel; }
        set
        {
            GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel = value;
        }
    }
    
    [Category(GardenTreasure)]
    [DisplayName("炸弹")]
    public int GardenBomb
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.GardenBomb); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
            
            if (value > 0)
            {
                UserData.Instance.SetRes(UserData.ResourceId.GardenBomb, value, reason);
            }
            else
            {
                UserData.Instance.ConsumeRes(UserData.ResourceId.GardenBomb, Math.Abs(value), reason);
            }
        }
    }
}