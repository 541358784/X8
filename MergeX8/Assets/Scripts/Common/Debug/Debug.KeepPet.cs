using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Activity.TreasureMap;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;


public partial class SROptions
{
    [Category(KeepPet)]
    [DisplayName("清除数据")]
    public void ClearStorage()
    {
        StorageManager.Instance.GetStorage<StorageHome>().KeepPet.Clear();
        KeepPetModel.Instance.InitConfig();
        var guideIdList = new List<int>() {};
        for (var i = 3021; i <= 3044; i++)
        {
            guideIdList.Add(i);
        }
        CleanGuideList(guideIdList);
    }
    [Category(KeepPet)]
    [DisplayName("显示狗子礼包 ")]
    public void ShowKeepPetPack()
    {
        HideDebugPanel();
        UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetGift, "debug");
    }
    [Category(KeepPet)]
    [DisplayName("设置巡逻任务剩余时间")]
    public string SetSearchTaskLeftTime
    {
        get
        {
            if (KeepPetModel.Instance.CurState.Enum != KeepPetStateEnum.Searching)
                return "0";
            var curTime = APIManager.Instance.GetServerTime();
            return (KeepPetModel.Instance.Storage.SearchEndTime - (long)curTime).ToString();
        }
        set
        {
            if (KeepPetModel.Instance.CurState.Enum != KeepPetStateEnum.Searching)
                return;
            var curTime = APIManager.Instance.GetServerTime();
            KeepPetModel.Instance.Storage.SearchEndTime = (long) curTime + long.Parse(value);
        }
    }
    [Category(KeepPet)]
    [DisplayName("设置飞盘")]
    public int SetKeepPetDogFrisbee
    {
        get
        {
            return UserData.Instance.GetRes(UserData.ResourceId.KeepPetDogFrisbee);
        }
        set
        {
            UserData.Instance.SetRes(UserData.ResourceId.KeepPetDogFrisbee,value,
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        }
    }
    [Category(KeepPet)]
    [DisplayName("设置鸡腿")]
    public int SetKeepPetDogDrumstick
    {
        get
        {
            return UserData.Instance.GetRes(UserData.ResourceId.KeepPetDogDrumstick);
        }
        set
        {
            UserData.Instance.SetRes(UserData.ResourceId.KeepPetDogDrumstick,value,
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        }
    }
    [Category(KeepPet)]
    [DisplayName("设置牛排")]
    public int SetKeepPetDogSteak
    {
        get
        {
            return UserData.Instance.GetRes(UserData.ResourceId.KeepPetDogSteak);
        }
        set
        {
            UserData.Instance.SetRes(UserData.ResourceId.KeepPetDogSteak,value,
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        }
    }
    [Category(KeepPet)]
    [DisplayName("设置狗头")]
    public int SetKeepPetDogHead
    {
        get
        {
            return UserData.Instance.GetRes(UserData.ResourceId.KeepPetDogHead);
        }
        set
        {
            UserData.Instance.SetRes(UserData.ResourceId.KeepPetDogHead,value,
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        }
    }
    [Category(KeepPet)]
    [DisplayName("设置经验值")]
    public int SetKeepPetExp
    {
        get
        {
            if (KeepPetModel.Instance.Storage == null)
                return 0;
            return KeepPetModel.Instance.Storage.Exp;
        }
        set
        {
            if (KeepPetModel.Instance.Storage == null)
                return;
            var oldValue = KeepPetModel.Instance.Storage.Exp;
            KeepPetModel.Instance.Storage.Exp = value;
            var newValue = KeepPetModel.Instance.Storage.Exp;
            KeepPetModel.Instance.UpdateDailyTaskOnExpChange(oldValue,newValue);
            EventDispatcher.Instance.SendEventImmediately(new EventKeepPetExpChange(oldValue, newValue));
            KeepPetModel.Instance.CheckLevelUpBi(oldValue, newValue);
        }
    }
     [Category(KeepPet)]
    [DisplayName("重置藏宝图 ")]
    public void ResetTreasureMap()
    {
        HideDebugPanel();
        TreasureMapModel.Instance.TreasureMap.Clear();
        
    }

    [Category(KeepPet)]
    [DisplayName("加一个藏宝图 ")]
    public void AddTreasureMap()
    {
        HideDebugPanel();
        TreasureMapModel.Instance.AddChip();
    }
    [Category(KeepPet)]
    [DisplayName("加一个线索 ")]
    public void Adds()
    {
        HideDebugPanel();
        UserData.Instance.AddRes(161,1,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        UserData.Instance.AddRes(162,1,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
    }

   
    public int _keepPetCollectType;
    [Category(KeepPet)]
    [DisplayName("类型")]
    public int KeepPetCollectType
    {
        get
        {
            return _keepPetCollectType;
        }
        set
        {
            _keepPetCollectType = value;
        }
    }

    public int _keepPetCollectCount;
    [Category(KeepPet)]
    [DisplayName("数量")]
    public int KeepPetCollectCount
    {
        get
        {
            return _keepPetCollectCount;
        }
        set
        {
            _keepPetCollectCount = value;
        }
    }

    [Category(KeepPet)]
    [DisplayName("消耗")]
    public void KeepPetConsume()
    {
        KeepPetModel.Instance.OnConsumeRes((UserData.ResourceId)_keepPetCollectType,_keepPetCollectCount);
    }
    [Category(KeepPet)]
    [DisplayName("获得")]
    public void KeepPetAddRes()
    {
        KeepPetModel.Instance.OnAddRes((UserData.ResourceId)_keepPetCollectType,_keepPetCollectCount);
    }

    private string abtestKey = "ABTEST_KEEPPET";
    [Category(KeepPet)] 
    [DisplayName("强制开启宠物")]
    public bool KeepAbTest
    {
        get
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(abtestKey))
            {
                string key = StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[abtestKey];
                return key == "1";
            }
            else
            {
                return false;
            }
        }
        set
        {
            if(value)
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[abtestKey] = "1";
            else
            {
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Remove(abtestKey);
            }
        }
    }
    
    [Category(KeepPet)]
    [DisplayName("饥饿日期")]
    public int KeepPetHungryDayId
    {
        get
        {
            return KeepPetModel.Instance.Storage.HungryDayId;
        }
        set
        {
            KeepPetModel.Instance.Storage.HungryDayId = value;
        }
    }

    [Category(KeepPet)]
    [DisplayName("生成鸡腿任务")]
    public void KeepPetAddOrder()
    {
        MainOrderCreateKeepPet.TryCreateOrder(SlotDefinition.KeepPet); 
    }
}
