using System.Collections.Generic;
using System.ComponentModel;
using Activity.FarmTimeOrder;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Farm.Model;
using Farm.Order;
using Gameplay;

public partial class SROptions
{
    private const string Farm = "农场";

    [Category(Farm)]
    [DisplayName("重置农场")]
    public void RestFarm()
    {
        CleanFarmGuide();
        FarmModel.Instance.storageFarm.Clear();
        StorageManager.Instance.GetStorage<StorageDecoration>().WorldMap.Remove(2);
    }
    
    [Category(Farm)]
    [DisplayName("农场等级")]
    public int FarmLevel
    {
        get
        {
            return FarmModel.Instance.storageFarm.Level;
        }
        set
        {
            FarmModel.Instance.storageFarm.Level = value;
        }
    }
    
    [Category(Farm)]
    [DisplayName("农场任务DEBUG")]
    public bool FarmOrder
    {
        get
        {
            return FarmModel.Instance.Debug_OpenModule;
        }
        set
        {
            FarmModel.Instance.Debug_OpenModule = value;
            EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_DEBUG_ORDER_OPEN);
        }
    }

    [Category(Farm)]
    [DisplayName("强制开启农场")]
    public bool OpenFarm
    {
        get
        {
            return FarmModel.Instance.Debug_OpenFram;
        }
        set
        {
            FarmModel.Instance.Debug_OpenFram = value;
        }
    }
    
    [Category(Farm)]
    [DisplayName("完成所有任务")]
    public bool CompleteAllOrder
    {
        get
        {
            return FarmModel.Instance.Debug_CompleteAllOrder;
        }
        set
        {
            FarmModel.Instance.Debug_CompleteAllOrder = value;
            EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_ORDER_REFRESH);
        }
        
    }
    
    private void CleanFarmGuide()
    {
        var guideIdList = new List<int>() {4431,4432,4433,4434,4435,4436,4437,4438,4439,4440,4441,4442,4443,4444};
        CleanGuideList(guideIdList);

        StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog.Remove(20000);
        StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog.Remove(21000);
        
        StorageManager.Instance.GetStorage<StorageHome>().StoryMovieData.FinishedId.Remove(10001);
        StorageManager.Instance.GetStorage<StorageHome>().StoryMovieData.FinishedId.Remove(10005);
        StorageManager.Instance.GetStorage<StorageHome>().StoryMovieData.FinishedId.Remove(10008);
        StorageManager.Instance.GetStorage<StorageHome>().StoryMovieData.FinishedId.Remove(10015);
        
    }
    
    [Category(Farm)]
    [DisplayName("增加扩充背包道具")]
    public void AddWareProductProp()
    {
        FarmModel.Instance.AddProductItem(9991, 10);
        FarmModel.Instance.AddProductItem(9992, 10);
        FarmModel.Instance.AddProductItem(9993, 10);
        FarmModel.Instance.AddProductItem(9994, 10);
    }
    
    [Category(Farm)]
    [DisplayName("增加农场加速道具")]
    public void AddSeedUpProp()
    {
        FarmModel.Instance.AddProductItem((int)UserData.ResourceId.Farm_SFertilizer, 10);
        FarmModel.Instance.AddProductItem((int)UserData.ResourceId.Farm_SKettle, 10);
        FarmModel.Instance.AddProductItem((int)UserData.ResourceId.Farm_Clock, 10);
        FarmModel.Instance.AddProductItem((int)UserData.ResourceId.Farm_Gear, 10);
    }
    
    [Category(Farm)]
    [DisplayName("重置限时任务")]
    public void RestFarmTimeOrder()
    {
        HideDebugPanel();
        FarmTimeLimitOrderModel.Instance.FarmTimeLimitOrder.Clear();
        
        for (var i = 0; i < FarmModel.Instance.storageFarm.Order.Orders.Count; i++)
        {
            if(FarmModel.Instance.storageFarm.Order.Orders[i].Slot != (int)OrderSlot.Activity_TimeOrder)
                continue;
            
            FarmModel.Instance.storageFarm.Order.Orders.RemoveAt(i);
            i--;
        }
    }
}