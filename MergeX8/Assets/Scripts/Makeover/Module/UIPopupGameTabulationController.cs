using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectLine;
using ConnectLine.Model;
using DG.Tweening;
using DigTrench;
using DragonPlus;
using DragonPlus.Config.DigTrench;
using DragonPlus.Config.FishEatFish;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using FishEatFishSpace;
using Gameplay;
using Makeover;
using OnePath;
using OnePathSpace;
using Stimulate.Configs;
using StimulateSpace;
using UnityEngine;
using UnityEngine.UI;
public partial class UIPopupGameTabulationController : UIWindowController
{
    public enum MiniGameTypeTab
    {
        None,
        MakeOver,
        DigTrench,
        FishEatFish,
        OnePath,
        ConnectLine,
        Stimulate,
        Psychology,
        
        Screw,
        Merge,
    }

    private List<TableMiniGameGroup> TableMiniGameGroupList => 
        StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.46")?
    GlobalConfigManager.Instance.TableMiniGameGroupNewConfig:
    GlobalConfigManager.Instance.TableMiniGameGroupConfig;
    private Dictionary<int,TableMiniGameItem> TableMiniGameItemList => GlobalConfigManager.Instance.TableMiniGameItemConfig;
    // private MiniGameTypeTab _miniGameType;
    // private GameObject _scrollView;
    // private GameObject _item;
    // private GameObject _rewardItem;
    private Button _closeBtn;
    // private GameObject _content;
    // private List<UITabulationItem> _items = new List<UITabulationItem>();
    private List<UITabulationItemAsmr> _itemsAsmr = new List<UITabulationItemAsmr>();
    private List<UITabulationItemDigTrench> _itemsDigTrench = new List<UITabulationItemDigTrench>();
    private List<UITabulationItemFishEatFish> _itemsFishEatFish = new List<UITabulationItemFishEatFish>();
    private List<UITabulationItemOnePath> _itemsOnePath = new List<UITabulationItemOnePath>();
    private List<UITabulationItemConnectLine> _itemsConnectLine = new List<UITabulationItemConnectLine>();
    private List<UITabulationItemStimulate> _itemsStimulate = new List<UITabulationItemStimulate>();
    public override void PrivateAwake()
    {
        if ((MiniGameTypeTab)StorageManager.Instance.GetStorage<StorageHome>().MiniGameDefaultType == MiniGameTypeTab.None)
        {
            StorageManager.Instance.GetStorage<StorageHome>().MiniGameDefaultType = (int) MiniGameTypeTab.DigTrench;
        }
        InitStorage();
        // _miniGameType = (MiniGameTypeTab) StorageManager.Instance.GetStorage<StorageHome>().MiniGameDefaultType;
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(() =>
        {
            ClickUIMask();
        });
        // _scrollView = GetItem("Root/Scroll View");
        //
        // _item = GetItem("Root/Scroll View/Viewport/Content/Level");
        // _item.gameObject.SetActive(false);
        // _rewardItem = GetItem("Root/Scroll View/Viewport/Content/Reward");
        // _rewardItem.gameObject.SetActive(false);
        //
        // _content = GetItem("Root/Scroll View/Viewport/Content");
        // InitView();
        // XUtility.WaitFrames(1, UpdateContentPosition);
        InitView();

        EventDispatcher.Instance.AddEventListener(EventEnum.ASMR_BEGIN_DOWNLOAD, Asmr_BeginDownLoad);
        EventDispatcher.Instance.AddEventListener(EventEnum.ASMR_DOWNLOAD_PROGRESS, Asmr_DownLoadProgress);
        EventDispatcher.Instance.AddEventListener(EventEnum.ASMR_End_DOWNLOAD, Asmr_EndDownLoad);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.DIG_TRENCH_BEGIN_DOWNLOAD, DigTrench_BeginDownLoad);
        EventDispatcher.Instance.AddEventListener(EventEnum.DIG_TRENCH_DOWNLOAD_PROGRESS, DigTrench_DownLoadProgress);
        EventDispatcher.Instance.AddEventListener(EventEnum.DIG_TRENCH_End_DOWNLOAD, DigTrench_EndDownLoad);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.FISH_EAT_FISH_BEGIN_DOWNLOAD, FishEatFish_BeginDownLoad);
        EventDispatcher.Instance.AddEventListener(EventEnum.FISH_EAT_FISH_DOWNLOAD_PROGRESS, FishEatFish_DownLoadProgress);
        EventDispatcher.Instance.AddEventListener(EventEnum.FISH_EAT_FISH_End_DOWNLOAD, FishEatFish_EndDownLoad);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.ONE_PATH_BEGIN_DOWNLOAD, OnePath_BeginDownLoad);
        EventDispatcher.Instance.AddEventListener(EventEnum.ONE_PATH_DOWNLOAD_PROGRESS, OnePath_DownLoadProgress);
        EventDispatcher.Instance.AddEventListener(EventEnum.ONE_PATH_End_DOWNLOAD, OnePath_EndDownLoad);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.CONNECT_LINE_BEGIN_DOWNLOAD, ConnectLine_BeginDownLoad);
        EventDispatcher.Instance.AddEventListener(EventEnum.CONNECT_LINE_DOWNLOAD_PROGRESS, ConnectLine_DownLoadProgress);
        EventDispatcher.Instance.AddEventListener(EventEnum.CONNECT_LINE_End_DOWNLOAD, ConnectLine_EndDownLoad);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.StimulateBeginDownload, Stimulate_BeginDownLoad);
        EventDispatcher.Instance.AddEventListener(EventEnum.StimulateDownloadProgress, Stimulate_DownLoadProgress);
        EventDispatcher.Instance.AddEventListener(EventEnum.StimulateEndDownload, Stimulate_EndDownLoad);
    }
    
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ASMR_BEGIN_DOWNLOAD, Asmr_BeginDownLoad);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ASMR_DOWNLOAD_PROGRESS, Asmr_DownLoadProgress);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ASMR_End_DOWNLOAD, Asmr_EndDownLoad);
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.DIG_TRENCH_BEGIN_DOWNLOAD, DigTrench_BeginDownLoad);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.DIG_TRENCH_DOWNLOAD_PROGRESS, DigTrench_DownLoadProgress);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.DIG_TRENCH_End_DOWNLOAD, DigTrench_EndDownLoad);
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FISH_EAT_FISH_BEGIN_DOWNLOAD, FishEatFish_BeginDownLoad);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FISH_EAT_FISH_DOWNLOAD_PROGRESS, FishEatFish_DownLoadProgress);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FISH_EAT_FISH_End_DOWNLOAD, FishEatFish_EndDownLoad);
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ONE_PATH_BEGIN_DOWNLOAD, OnePath_BeginDownLoad);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ONE_PATH_DOWNLOAD_PROGRESS, OnePath_DownLoadProgress);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ONE_PATH_End_DOWNLOAD, OnePath_EndDownLoad);
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.CONNECT_LINE_BEGIN_DOWNLOAD, ConnectLine_BeginDownLoad);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.CONNECT_LINE_DOWNLOAD_PROGRESS, ConnectLine_DownLoadProgress);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.CONNECT_LINE_End_DOWNLOAD, ConnectLine_EndDownLoad);
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.StimulateBeginDownload, Stimulate_BeginDownLoad);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.StimulateDownloadProgress, Stimulate_DownLoadProgress);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.StimulateEndDownload, Stimulate_EndDownLoad);
    }
    public override void ClickUIMask()
    {
        // base.ClickUIMask();
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow(() =>
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
        });
    }

    #region ASMR
    private void Asmr_BeginDownLoad(BaseEvent e)
    {
        TableMoLevel config = (TableMoLevel)e.datas[0];
        _itemsAsmr.ForEach(a=>a.BeginDownLoad(config.id));
    }
    private void Asmr_EndDownLoad(BaseEvent e)
    {
        TableMoLevel config = (TableMoLevel)e.datas[0];
        _itemsAsmr.ForEach(a=>a.EndDownLoad(config.id));
    }
    private void Asmr_DownLoadProgress(BaseEvent e)
    {
        TableMoLevel config = (TableMoLevel)e.datas[0];
        float progress = (float)e.datas[1];
        _itemsAsmr.ForEach(a=>a.DownLoadProgress(config.id, progress));
    }
    #endregion

    #region DigTrench
    private void DigTrench_BeginDownLoad(BaseEvent e)
    {
        DigTrenchLevel config = (DigTrenchLevel)e.datas[0];
        _itemsDigTrench.ForEach(a=>a.BeginDownLoad(config.id));
    }
    private void DigTrench_EndDownLoad(BaseEvent e)
    {
        DigTrenchLevel config = (DigTrenchLevel)e.datas[0];
        _itemsDigTrench.ForEach(a=>a.EndDownLoad(config.id));
    }
    private void DigTrench_DownLoadProgress(BaseEvent e)
    {
        DigTrenchLevel config = (DigTrenchLevel)e.datas[0];
        float progress = (float)e.datas[1];
        _itemsDigTrench.ForEach(a=>a.DownLoadProgress(config.id, progress));
    }
    #endregion

    #region FishEatFish
    private void FishEatFish_BeginDownLoad(BaseEvent e)
    {
        FishEatFishLevel config = (FishEatFishLevel)e.datas[0];
        _itemsFishEatFish.ForEach(a=>a.BeginDownLoad(config.id));
    }
    private void FishEatFish_EndDownLoad(BaseEvent e)
    {
        FishEatFishLevel config = (FishEatFishLevel)e.datas[0];
        _itemsFishEatFish.ForEach(a=>a.EndDownLoad(config.id));
    }
    private void FishEatFish_DownLoadProgress(BaseEvent e)
    {
        FishEatFishLevel config = (FishEatFishLevel)e.datas[0];
        float progress = (float)e.datas[1];
        _itemsFishEatFish.ForEach(a=>a.DownLoadProgress(config.id, progress));
    }
    #endregion
    
    #region OnePath

    private void OnePath_BeginDownLoad(BaseEvent e)
    {
        TableOnePathLevel config = (TableOnePathLevel)e.datas[0];
        _itemsOnePath.ForEach(a=>a.BeginDownLoad(config.id));
    }
    private void OnePath_EndDownLoad(BaseEvent e)
    {
        TableOnePathLevel config = (TableOnePathLevel)e.datas[0];
        _itemsOnePath.ForEach(a=>a.EndDownLoad(config.id));
    }
    private void OnePath_DownLoadProgress(BaseEvent e)
    {
        TableOnePathLevel config = (TableOnePathLevel)e.datas[0];
        float progress = (float)e.datas[1];
        _itemsOnePath.ForEach(a=>a.DownLoadProgress(config.id, progress));
    }
    #endregion

    #region ConnectLine
    private void ConnectLine_BeginDownLoad(BaseEvent e)
    {
        TableConnectLineLevel config = (TableConnectLineLevel)e.datas[0];
        _itemsConnectLine.ForEach(a=>a.BeginDownLoad(config.id));
    }
    private void ConnectLine_EndDownLoad(BaseEvent e)
    {
        TableConnectLineLevel config = (TableConnectLineLevel)e.datas[0];
        _itemsConnectLine.ForEach(a=>a.EndDownLoad(config.id));
    }
    private void ConnectLine_DownLoadProgress(BaseEvent e)
    {
        TableConnectLineLevel config = (TableConnectLineLevel)e.datas[0];
        float progress = (float)e.datas[1];
        _itemsConnectLine.ForEach(a=>a.DownLoadProgress(config.id, progress));
    }
    #endregion
    
    #region Stimulate
    private void Stimulate_BeginDownLoad(BaseEvent e)
    {
        TableStimulateSetting config = (TableStimulateSetting)e.datas[0];
        _itemsStimulate.ForEach(a=>a.BeginDownLoad(config.id));
    }
    private void Stimulate_EndDownLoad(BaseEvent e)
    {
        TableStimulateSetting config = (TableStimulateSetting)e.datas[0];
        _itemsStimulate.ForEach(a=>a.EndDownLoad(config.id));
    }
    private void Stimulate_DownLoadProgress(BaseEvent e)
    {
        TableStimulateSetting config = (TableStimulateSetting)e.datas[0];
        float progress = (float)e.datas[1];
        _itemsStimulate.ForEach(a=>a.DownLoadProgress(config.id, progress));
    }
    #endregion

    // public void InitView()
    // {
    //     int index = 0;
    //     for (var i = 0; i < TableMiniGameGroupList.Count; i++)
    //     {
    //         var group = TableMiniGameGroupList[i];
    //         var itemConfigList = new List<TableMiniGameItem>();
    //         for (var i1 = 0; i1 < group.itemList.Length; i1++)
    //         {
    //             var item = TableMiniGameItemList[group.itemList[i1]];
    //             var i2 = 0;
    //             for (i2 = 0; i2 < itemConfigList.Count; i2++)
    //             {
    //                 if (!CompareOrderUnder(item, itemConfigList[i2]))
    //                 {
    //                     break;
    //                 }
    //             }
    //             itemConfigList.Insert(i2,item);
    //         }
    //
    //         var startIndex = index;
    //         for (var i1 = 0; i1 < itemConfigList.Count; i1++)
    //         {
    //             var item = itemConfigList[i1];
    //             var cloneObj = Instantiate(_item, _content.transform);
    //             cloneObj.gameObject.SetActive(true);
    //             var typeTab = (MiniGameTypeTab) item.configType;
    //             
    //             UITabulationItem itemData = null;
    //             if (typeTab == MiniGameTypeTab.DigTrench)
    //             {
    //                 itemData = new UITabulationItemDigTrench();
    //                 _itemsDigTrench.Add(itemData as UITabulationItemDigTrench);
    //             }
    //             else if (typeTab == MiniGameTypeTab.FishEatFish)
    //             {
    //                 itemData = new UITabulationItemFishEatFish();
    //                 _itemsFishEatFish.Add(itemData as UITabulationItemFishEatFish);
    //             }
    //             else if (typeTab == MiniGameTypeTab.OnePath)
    //             {
    //                 itemData = new UITabulationItemOnePath();
    //                 _itemsOnePath.Add(itemData as UITabulationItemOnePath);
    //             }
    //             else if (typeTab == MiniGameTypeTab.ConnectLine)
    //             {
    //                 itemData = new UITabulationItemConnectLine();
    //                 _itemsConnectLine.Add(itemData as UITabulationItemConnectLine);
    //             }
    //             itemData.Init(cloneObj, item.configId, index++);
    //             _items.Add(itemData);
    //         }
    //
    //         {
    //             var rewardObj = Instantiate(_rewardItem, _content.transform);
    //             rewardObj.gameObject.SetActive(true);
    //             RewardItem rewardItem = new RewardItem();
    //             rewardItem.Init(rewardObj,group,startIndex,index);
    //         }
    //     }
    //
    //     {
    //         //结尾加一个comingSoon
    //         var comingObj = Instantiate(_item, _content.transform);
    //         comingObj.gameObject.SetActive(true);
    //         UITabulationItemDigTrench item = new UITabulationItemDigTrench();
    //         item.Init(comingObj, -1, index++);
    //     }
    // }
    // public void UpdateContentPosition()
    // {
    //     
    //     GameObject firstItem = _items[0]._gameObject;
    //     for (var i = 0; i < _items.Count; i++)
    //     {
    //         if (!_items[i].IsFinish())
    //         {
    //             if (i > 0)
    //                 firstItem = _items[i-1]._gameObject;
    //             break;
    //         }
    //     }
    //     if (firstItem == null)
    //         return;
    //
    //     _scrollView.GetComponent<ScrollRect>().enabled = false;
    //     
    //     RectTransform rectContent = (RectTransform)_content.transform;
    //     float maxHeight = rectContent.rect.height;
    //     float moveHeight = -firstItem.transform.localPosition.y - ((RectTransform)(firstItem.transform)).rect.height/2;
    //     moveHeight = Mathf.Min(maxHeight, moveHeight);
    //     
    //     rectContent.DOAnchorPosY(moveHeight, 0f);
    //     XUtility.WaitFrames(1, () =>
    //     {
    //         _scrollView.GetComponent<ScrollRect>().enabled = true; 
    //     });
    // }

    public static int GetUnlockNeedTaskCount(int miniGameType,int configId)
    {
        var typeTab = (MiniGameTypeTab) miniGameType;
        if (typeTab == MiniGameTypeTab.DigTrench)
        {
            var config = DigTrenchConfigManager.Instance.DigTrenchLevelList.Find(a => a.id == configId);
            return config.unlockNodeNum;
        }
        else if (typeTab == MiniGameTypeTab.FishEatFish)
        {
            var config = FishEatFishConfigManager.Instance.FishEatFishLevelList.Find(a => a.id == configId);
            return config.unlockNodeNum;
        }
        else if (typeTab == MiniGameTypeTab.OnePath)
        {
            var config = OnePathConfigManager.Instance._configs.Find(a => a.id == configId);
            return config.unlockNodeNum;
        }
        else if (typeTab == MiniGameTypeTab.ConnectLine)
        {
            var config = ConnectLineConfigManager.Instance._configs.Find(a => a.id == configId);
            return config.unlockNodeNum;
        }
        return 0;
    }

    public static bool IsFinish(int miniGameType,int configId)
    {
        var typeTab = (MiniGameTypeTab) miniGameType;
        if (typeTab == MiniGameTypeTab.DigTrench)
        {
            var config = DigTrenchConfigManager.Instance.DigTrenchLevelList.Find(a => a.id == configId);
            return DigTrenchEntryControllerModel.Instance.IsFinish(config);
        }
        else if (typeTab == MiniGameTypeTab.FishEatFish)
        {
            var config = FishEatFishConfigManager.Instance.FishEatFishLevelList.Find(a => a.id == configId);
            return FishEatFishEntryControllerModel.Instance.IsFinish(config);
        }
        else if (typeTab == MiniGameTypeTab.OnePath)
        {
            var config = OnePathConfigManager.Instance._configs.Find(a => a.id == configId);
            return OnePathEntryControllerModel.Instance.IsFinish(config);
        }
        else if (typeTab == MiniGameTypeTab.ConnectLine)
        {
            var config = ConnectLineConfigManager.Instance._configs.Find(a => a.id == configId);
            return ConnectLineModel.Instance.IsFinish(config);
        }
        else if (typeTab == MiniGameTypeTab.Stimulate)
        {
            var config = StimulateConfigManager.Instance._stimulateSetting.Find(a => a.id == configId);
            return StimulateEntryControllerModel.Instance.IsFinish(config);
        }
        return false;
    }

    public void InitStorage()
    {
        if (StorageManager.Instance.GetStorage<StorageHome>().MiniGame.IsInit)
        {
            return;
        }
        StorageManager.Instance.GetStorage<StorageHome>().MiniGame.IsInit = true;
        int index = 0;
        for (var i = 0; i < TableMiniGameGroupList.Count; i++)
        {
            var group = TableMiniGameGroupList[i];
            if (CanCollect(group))
            {
                StorageManager.Instance.GetStorage<StorageHome>().MiniGame.FinishGroupList.Add(group.id);
            }
        }
    }
    public static void CollectGroup(TableMiniGameGroup group)
    {
        if (!CanCollect(group))
            return;
        StorageManager.Instance.GetStorage<StorageHome>().MiniGame.FinishGroupList.Add(group.id);
        var rewardList = CommonUtils.FormatReward(group.rewardId, group.rewardNum);
        var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MinigameCommonGet
        };
        CommonRewardManager.Instance.PopCommonReward(rewardList,CurrencyGroupManager.Instance.GetCurrencyUseController(), true,
            reasonArgs, () =>
        {
        });
        
    }
    public static bool CanCollect(TableMiniGameGroup group)
    {
        if (IsCollected(group))
            return false;
        return IsFinishAllLevel(group);
    }

    public static bool IsFinishAllLevel(TableMiniGameGroup group)
    {
        for (var i = 0; i < group.itemList.Length; i++)
        {
            var itemConfig = GlobalConfigManager.Instance.TableMiniGameItemConfig[group.itemList[i]];
            if (!IsFinish(itemConfig.configType, itemConfig.configId))
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsCollected(TableMiniGameGroup group)
    {
        return StorageManager.Instance.GetStorage<StorageHome>().MiniGame.FinishGroupList
            .Contains(group.id);
    }

    public static bool CompareOrderUnder(TableMiniGameItem a,TableMiniGameItem b)
    {
        var unlockA = GetUnlockNeedTaskCount(a.configType, a.configId);
        var unlockB = GetUnlockNeedTaskCount(b.configType, b.configId);
        if (unlockA == unlockB)
        {
            var selectMiniGameType = StorageManager.Instance.GetStorage<StorageHome>().MiniGameDefaultType;
            var typeA = a.configType;
            var typeB = b.configType;
            if (typeA == typeB)
            {
                return true;
            }
            else
            {
                if (typeA == selectMiniGameType)
                    return false;
                else if (typeB == selectMiniGameType)
                    return true;
                return typeA > typeB;
            }
        }
        else
            return unlockA > unlockB;
    }
}