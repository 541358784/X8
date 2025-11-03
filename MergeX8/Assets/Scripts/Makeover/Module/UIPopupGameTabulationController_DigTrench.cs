using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigTrench;
using DragonPlus.Config.DigTrench;
using DragonU3DSDK.Asset;
using UnityEngine;

public class UITabulationCellDigTrench:UITabulationCell
{
    private DigTrenchLevel _levelConfig;
    public override int GetUnlockDecoNodeNum()
    {
        return _levelConfig.unlockNodeNum;
    }

    public override void OnClickPlayBtn()
    {
        SceneFsm.mInstance.ChangeState(StatusType.DigTrench, _levelConfig,false);
    }

    public override Sprite GetIcon()
    {
        return ResourcesManager.Instance.GetSpriteVariant("DigTrenchAtlas", _levelConfig.icon);
    }

    public override Dictionary<string, string> GetNeedDownloadAssets()
    {
        return DigTrench.Model.Instance.GetNeedDownloadAssets(_levelConfig.levelResType);
    }

    public override void SetConfig(int configId)
    {
        _levelConfig = DigTrenchConfigManager.Instance.DigTrenchLevelList.Find(a => a.id == configId);
    }

    public override bool IsUnlock()
    {
        return DigTrenchEntryControllerModel.Instance.IsUnLock(_levelConfig);
    }

    public override bool IsFinish()
    {
        return DigTrenchEntryControllerModel.Instance.IsFinish(_levelConfig);
    }

    public override bool NeedUpdateOnDownload(int configId)
    {
        var config = DigTrenchConfigManager.Instance.DigTrenchLevelList.Find(a => a.id == configId);
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
            EventDispatcher.Instance.DispatchEvent(EventEnum.DIG_TRENCH_End_DOWNLOAD, _levelConfig);
        } );
        EventDispatcher.Instance.DispatchEvent(EventEnum.DIG_TRENCH_BEGIN_DOWNLOAD, _levelConfig);
    }
    public async void DownloadAssets(int levelId, Action<bool> onFinished)
    {
        var assets = DigTrench.Model.Instance.GetNeedDownloadAssets(levelId);
        if (assets == null || assets.Count == 0)
        {
            onFinished?.Invoke(true);
            return;
        }
            
        var downLoadFinish = false;
        var downloadTaskList = DigTrench.Model.Instance.DownloadFiles(assets, (success) => downLoadFinish = true);
    
        while (!downLoadFinish)
        {
            DigTrench.Model.Instance.UpdateProgressFromDownloadInfoList(downloadTaskList, (progress,extralInfo)=>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.DIG_TRENCH_DOWNLOAD_PROGRESS, _levelConfig, progress);
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
public class UITabulationItemDigTrench:UITabulationItem
{
    public override bool IsUnlock()
    {
        return DigTrenchEntryControllerModel.Instance.IsUnLock(_levelConfig);
    }
    public override bool IsFinish()
    {
        return DigTrenchEntryControllerModel.Instance.IsFinish(_levelConfig);
    }
    private DigTrenchLevel _levelConfig;
    public override void SetConfig(int configId)
    {
        _levelConfig = DigTrenchConfigManager.Instance.DigTrenchLevelList.Find(a => a.id == configId);
    }

    public override void CreateCell()
    {
        _normalCell = new UITabulationCellDigTrench();
        _lockCell = new UITabulationCellDigTrench();
        _comingsoonCell = new UITabulationCellDigTrench();
    }
}