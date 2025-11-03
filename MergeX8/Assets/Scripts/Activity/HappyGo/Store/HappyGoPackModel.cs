using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.HappyGo;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;

public enum PackEnum
{
    Build=1,
    Energy=2,
    Time=3
}
public class HappyGoPackModel : Manager<HappyGoPackModel>
{
    public  StorageDictionary<int,StorageHGBundleItem> packData
    {
        get { return StorageManager.Instance.GetStorage<StorageGame>().HappyGo.Bundles; }
    }

    public bool CanRefresh()
    {
        bool result = false;
        ulong now = APIManager.Instance.GetServerTime() / 1000;
        int nowTime = (int) (now) / (24 * 3600);
        int lastTime = (int) (HappyGoModel.Instance.storageHappy.BundleRefreshTime / (24 * 3600));

        DateTime serNow = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(now);
        DateTime refreshTime1 = serNow.AddDays(1);
        DateTime refreshTime2 = new DateTime(refreshTime1.Year, refreshTime1.Month, refreshTime1.Day, 0, 0, 0);
        int cp = DateTime.Compare(serNow, refreshTime2);
        if (cp > 0) // t1 大于t2
        {
           
        }
        else
        {
            result = nowTime > lastTime; //不是同一天
            
        }
        return result;
    }
    
    private int GetPackLeftSeconds()
    {
        ulong severTime = APIManager.Instance.GetServerTime() / 1000;
        DateTime serNow = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(severTime);
        DateTime refreshTime1 = serNow.AddDays(1);
        DateTime refreshTime2 = new DateTime(refreshTime1.Year, refreshTime1.Month, refreshTime1.Day, 0, 0, 0);
        TimeSpan span = refreshTime2 - serNow;
        return (int)span.TotalSeconds;
       
    }

    public string GetPackLeftTimeStr()
    {
        var time=  GetPackLeftSeconds();
        return TimeUtils.GetTimeString(time);
    }

    public StorageHGBundleItem GetBundleByType(PackEnum type)
    {
        if (packData.ContainsKey((int) type))
        {
            return packData[(int) type];
        }
        return null;
    }
    public StorageDictionary<int, StorageHGBundleItem> GetHappyGoBundleItems()
    {
        GenHappyGoBundle();
        return packData;
    }
    
    public void GenHappyGoBundle()
    {
        if (CanRefresh())
        {
            HappyGoModel.Instance.storageHappy.BundleRefreshTime =(long)( APIManager.Instance.GetServerTime() / 1000);
            packData.Clear();
            if (!HappyGoModel.Instance.storageHappy.IsBuyBuild)
            {
                var buildPack=  HappyGoModel.Instance.HGVDBundleList.Find(a => a.type == (int)PackEnum.Build);
                var buildPackStorage=GenPack(buildPack);
                packData.Add( (int)PackEnum.Build,buildPackStorage);
            }
          
            
            var energyPack=  HappyGoModel.Instance.HGVDBundleList.FindAll(a => a.type == (int)PackEnum.Energy);
            var energyPackStorage=GenPack(energyPack[0]);
            packData.Add( (int)PackEnum.Energy,energyPackStorage);       
            
            var tmePack=  HappyGoModel.Instance.HGVDBundleList.FindAll(a => a.type == (int)PackEnum.Time);
            var tmePackStorage=GenPack(tmePack[0]);
            packData.Add( (int)PackEnum.Time,tmePackStorage);
        }
    }

    public StorageHGBundleItem GenPack(HGVDBundle bundle)
    {
        StorageHGBundleItem item = new StorageHGBundleItem();
        item.PackId = bundle.id;
        item.LeftBuyCount = bundle.limit;
        item.ShopId = bundle.shopItemId;
        return item;
    }
    
 
    public int GetCanBuyCount()
    {
        return packData.Count;
    }

    public void RecordBuy(HGVDBundle bundle)
    {
        if (bundle!=null&&bundle.type == 1)
        {
            HappyGoModel.Instance.storageHappy.IsBuyBuild = true;
        }
        if (packData != null && packData.Count > 0)
        {
            int packKey = 0;
            foreach (var key in packData.Keys)
            {
                if (packData[key].PackId == bundle.id)
                {
                    packKey = key;
                }
            }

            if (packKey > 0)
            {
                packData[packKey].LeftBuyCount--;
                if (packData[packKey].LeftBuyCount <= 0)
                {
                    //寻找下个BUNDLE
                    var next = HappyGoModel.Instance.GetTableHgBundleById(bundle.id + 1);
                    if (next != null)
                    {
                        packData[packKey] = GenPack(next);
                    }
                    else
                    {
                        packData.Remove(packKey);
                    }
                }

            }
        }
    }

    public void PurchaseSuccess(TableShop item)
    {
        var bundle=  HappyGoModel.Instance.GetTableHgBundle(item.id);
        if (bundle != null)
        {
            var ret= CommonUtils.FormatReward(bundle.bundleItemList, bundle.bundleItemCountList);
            var reasonArgs =
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.HgVdBuyPackage);


            CommonRewardManager.Instance.PopHappyGoReward(ret, CurrencyGroupManager.Instance.currencyController, true,
                reasonArgs, () => {  });
            foreach (var res in ret)
            {
                if (!UserData.Instance.IsResource(res.id))
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                    if (itemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeReasonHgVdBuyPackage,
                            itemAId = itemConfig.id,
                            isChange = true,
                        });
                    }
                }
            }
            RecordBuy(bundle);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HAPPYGOBUNDLE_PURCHASE_REFRESH);

        }
        
    }


    public static bool CanShowPack()
    {
        Instance.GetHappyGoBundleItems();

        List<Action> actions = new List<Action>();
        if (HappyGoModel.Instance.GetLevel() >= GlobalConfigManager.Instance.GetNumValue("happy_go_energypack_unlock"))
        {
            var energy = Instance.GetBundleByType(PackEnum.Energy);
            if(energy!=null &&energy.LeftBuyCount>0)
            {
                actions.Add(() =>
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoGift2,energy);
                });
            }
        }

        if (HappyGoModel.Instance.GetLevel() >= GlobalConfigManager.Instance.GetNumValue("happy_go_boosterpack_unlock"))
        {
            var time = Instance.GetBundleByType(PackEnum.Time);
            if(time!=null&&time.LeftBuyCount>0)
            {
                actions.Add(() =>
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoGift2,time);
                });
          
            }
        }
        
       
        if (HappyGoModel.Instance.GetLevel() >= GlobalConfigManager.Instance.GetNumValue("happy_go_camerapack_unlock"))
        {
            var build = Instance.GetBundleByType(PackEnum.Build);
            if(build!=null&&build.LeftBuyCount>0)
            {
                actions.Add(() =>
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoGift1,build);
                });
            } 
        }

        if (actions.Count > 0)
        {
            if (HappyGoModel.Instance.storageHappy.PackPopIndex > actions.Count-1)
            {
                HappyGoModel.Instance.storageHappy.PackPopIndex = 0;
            }
            actions[HappyGoModel.Instance.storageHappy.PackPopIndex].Invoke();
            HappyGoModel.Instance.storageHappy.PackPopIndex++;
        }
        return false;
    }

    public void TryShowEnergyPack()
    {
        if (HappyGoModel.Instance.GetLevel() >= GlobalConfigManager.Instance.GetNumValue("happy_go_energypack_unlock"))
        {
            var energy = Instance.GetBundleByType(PackEnum.Energy);
            if(energy!=null)
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoGift2,energy);
            }
        }
    }

}
