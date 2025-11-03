using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using OutsideGuide;
using UnityEngine;

using TMatch;
public partial class SROptions
{
    [Category(TMatch)]
    [DisplayName("重置")]
    public void ResetTMatch()
    {
        foreach (var itemConfig in DragonPlus.Config.TMatchShop.TMatchShopConfigManager.Instance.ItemConfigList)
        {
            if (itemConfig.id > 100000 && itemConfig.id <= 101000)
                ItemModel.Instance.Clear(itemConfig.id,
                    new DragonPlus.GameBIManager.ItemChangeReasonArgs
                        {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug});
        }

        for (var i = 10112; i <= 10125; i++)
        {
            DecoGuideManager.Instance.ClearGameGuide(i);
        }

        StorageManager.Instance.GetStorage<StorageTMatch>().Clear();
        StorageManager.Instance.GetStorage<StorageTMatch>().LevelChest.TotalLevel = 1;
        StorageManager.Instance.GetStorage<StorageCurrencyTMatch>().Clear();
        StorageManager.Instance.GetStorage<StorageDecorationGuide>().Clear();
    }
    [Category(TMatch)]
    [DisplayName("改等级")]
    public int SetLevel
    {
        get
        {
            return TMatchModel.Instance.storageTMatch.MainLevel;
        }
        set
        {
            TMatchModel.Instance.storageTMatch.MainLevel = value;
            TMatchModel.Instance.storageTMatch.LevelChest.TotalLevel = value%10;
            TMatchModel.Instance.storageTMatch.LevelChest.CurIndex = value/10;
            global::TMatch.EventDispatcher.Instance.DispatchEvent(global::TMatch.EventEnum.TMATCH_GAME_WIN);
        }
    }
    
    [Category(TMatch)]
    [DisplayName("完成关卡")]
    public void FinishTMGame()
    {
        HideDebugPanel();
        global::TMatch.EventDispatcher.Instance.DispatchEvent(global::TMatch.EventEnum.TMATCH_GAME_TARGET_FINISH);
    }
    
    [Category(TMatch)]
    [DisplayName("加周挑战分数")]
    public int AddWeekChallengeCore
    {
        get
        {
            return 0;
        }
        set
        {
            global::TMatch.EventDispatcher.Instance.DispatchEventImmediately(
                new WeeklyChellengeAddCollectCntEvent(value));
        }
    }
    
    [Category(TMatch)]
    [DisplayName("改周挑战分数")]
    public int SetWeekChallengeCore
    {
        get
        {
            return WeeklyChallengeController.Instance.model != null?WeeklyChallengeController.Instance.model.stoage.CurCollectItemNum:0;
        }
        set
        {
            if (WeeklyChallengeController.Instance.model == null)
                return;
            WeeklyChallengeController.Instance.model.stoage.CurCollectItemNum = value;
            global::TMatch.EventDispatcher.Instance.DispatchEventImmediately(
                new WeeklyChellengeAddCollectCntEvent(0));
        }
    }
    
    [Category(TMatch)]
    [DisplayName("重置TM鳄鱼")]
    public void ResetTMatchCrocodile()
    {
        HideDebugPanel();
        CrocodileActivityModel.Instance.Storage.Clear();
    }
    
    [Category(TMatch)]
    [DisplayName("当前关layout")]
    public int TMatchCurrentLayout
    {
        get
        {
            if (TMatchSystem.LevelController!=null)
                return TMatchSystem.LevelController.LevelData.layoutCfg.id;
            return -1;
        }
    }
}