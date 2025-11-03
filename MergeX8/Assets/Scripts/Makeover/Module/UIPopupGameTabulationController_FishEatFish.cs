using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishEatFishSpace;
using DragonPlus.Config.FishEatFish;
using DragonU3DSDK.Asset;
using UnityEngine;

public class UITabulationCellFishEatFish:UITabulationCell
{
    private FishEatFishLevel _levelConfig;
    public override int GetUnlockDecoNodeNum()
    {
        return _levelConfig.unlockNodeNum;
    }

    public override void OnClickPlayBtn()
    {
        SceneFsm.mInstance.ChangeState(StatusType.FishEatFish, _levelConfig,false);
    }

    public override Sprite GetIcon()
    {
        return ResourcesManager.Instance.GetSpriteVariant("FishEatFishAtlas", _levelConfig.icon);
    }

    public override Dictionary<string, string> GetNeedDownloadAssets()
    {
        return FishEatFishSpace.Model.Instance.GetNeedDownloadAssets(_levelConfig.levelResType);
    }

    public override void SetConfig(int configId)
    {
        _levelConfig = FishEatFishConfigManager.Instance.FishEatFishLevelList.Find(a => a.id == configId);
    }

    public override bool IsUnlock()
    {
        return FishEatFishEntryControllerModel.Instance.IsUnLock(_levelConfig);
    }

    public override bool IsFinish()
    {
        return FishEatFishEntryControllerModel.Instance.IsFinish(_levelConfig);
    }

    public override bool NeedUpdateOnDownload(int configId)
    {
        var config = FishEatFishConfigManager.Instance.FishEatFishLevelList.Find(a => a.id == configId);
        if(_levelConfig == null || _levelConfig == config)
            return false;
        if(_levelConfig.levelResType != config.levelResType)
            return false;
        return true;
    }

    public override void OnClickDownloadBtn()
    {
        DownloadAssets(_levelConfig.levelResType, b =>
        {
            UpdateUI();
            EventDispatcher.Instance.DispatchEvent(EventEnum.FISH_EAT_FISH_End_DOWNLOAD, _levelConfig);
        } );
        EventDispatcher.Instance.DispatchEvent(EventEnum.FISH_EAT_FISH_BEGIN_DOWNLOAD, _levelConfig);
    }
    public async void DownloadAssets(int levelId, Action<bool> onFinished)
    {
        var assets = FishEatFishSpace.Model.Instance.GetNeedDownloadAssets(levelId);
        if (assets == null || assets.Count == 0)
        {
            onFinished?.Invoke(true);
            return;
        }
            
        var downLoadFinish = false;
        var downloadTaskList = FishEatFishSpace.Model.Instance.DownloadFiles(assets, (success) => downLoadFinish = true);
    
        while (!downLoadFinish)
        {
            FishEatFishSpace.Model.Instance.UpdateProgressFromDownloadInfoList(downloadTaskList, (progress,extralInfo)=>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.FISH_EAT_FISH_DOWNLOAD_PROGRESS, _levelConfig, progress);
                updateProgress(progress, extralInfo);
            });
            await Task.Delay(100);
        }
    
        foreach (var info in downloadTaskList)
        {
            if (info.result != DownloadResult.Success)
            {
                onFinished?.Invoke(false);
                return;
            }
        }
        onFinished?.Invoke(true);
    }
}
public class UITabulationItemFishEatFish:UITabulationItem
{
    public override bool IsUnlock()
    {
        return FishEatFishEntryControllerModel.Instance.IsUnLock(_levelConfig);
    }
    public override bool IsFinish()
    {
        return FishEatFishEntryControllerModel.Instance.IsFinish(_levelConfig);
    }
    private FishEatFishLevel _levelConfig;
    public override void SetConfig(int configId)
    {
        _levelConfig = FishEatFishConfigManager.Instance.FishEatFishLevelList.Find(a => a.id == configId);
    }

    public override void CreateCell()
    {
        _normalCell = new UITabulationCellFishEatFish();
        _lockCell = new UITabulationCellFishEatFish();
        _comingsoonCell = new UITabulationCellFishEatFish();
    }
}