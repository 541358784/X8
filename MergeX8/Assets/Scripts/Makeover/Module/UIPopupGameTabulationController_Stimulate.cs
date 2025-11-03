using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stimulate;
using DragonU3DSDK.Asset;
using Stimulate.Configs;
using StimulateSpace;
using UnityEngine;

public class UITabulationCellStimulate:UITabulationCell
{
    private TableStimulateSetting _levelConfig;
    public override int GetUnlockDecoNodeNum()
    {
        return _levelConfig.unlockNodeNum;
    }

    public override void OnClickPlayBtn()
    {
        //SceneFsm.mInstance.ChangeState(StatusType.EnterStimulate, _levelConfig,false);
        SceneFsm.mInstance.ChangeState(StatusType.EnterStimulate, StimulateConfigManager.Instance._stimulateSetting[0],false);
    } 

    public override Sprite GetIcon()
    {
        return ResourcesManager.Instance.GetSpriteVariant("StimulateAtlas", _levelConfig.icon);
    }

    public override Dictionary<string, string> GetNeedDownloadAssets()
    {
        return StimulateSpace.Model.Instance.GetNeedDownloadAssets(_levelConfig.levelId);
    }

    public override void SetConfig(int configId)
    {
        _levelConfig = StimulateConfigManager.Instance._stimulateSetting.Find(a => a.id == configId);
    }

    public override bool IsUnlock()
    {
        return StimulateEntryControllerModel.Instance.IsUnLock(_levelConfig);
    }

    public override bool IsFinish()
    {
        return StimulateEntryControllerModel.Instance.IsFinish(_levelConfig);
    }

    public override bool NeedUpdateOnDownload(int configId)
    {
        var config = StimulateConfigManager.Instance._stimulateSetting.Find(a => a.id == configId);
        if(_levelConfig == null || _levelConfig == config)
            return false;
        if(_levelConfig.levelId != config.levelId)
            return false;
        return true;
    }

    public override void OnClickDownloadBtn()
    {
        DownloadAssets(_levelConfig.levelId, b =>
        {
            UpdateUI();
            EventDispatcher.Instance.DispatchEvent(EventEnum.StimulateEndDownload, _levelConfig);
        } );
        EventDispatcher.Instance.DispatchEvent(EventEnum.StimulateBeginDownload, _levelConfig);
    }
    public async void DownloadAssets(int levelId, Action<bool> onFinished)
    {
        var assets = StimulateSpace.Model.Instance.GetNeedDownloadAssets(levelId);
        if (assets == null || assets.Count == 0)
        {
            onFinished?.Invoke(true);
            return;
        }
            
        var downLoadFinish = false;
        var downloadTaskList = StimulateSpace.Model.Instance.DownloadFiles(assets, (success) => downLoadFinish = true);
    
        while (!downLoadFinish)
        {
            StimulateSpace.Model.Instance.UpdateProgressFromDownloadInfoList(downloadTaskList, (progress,extralInfo)=>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.StimulateDownloadProgress, _levelConfig, progress);
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
public class UITabulationItemStimulate:UITabulationItem
{
    public override bool IsUnlock()
    {
        return StimulateEntryControllerModel.Instance.IsUnLock(_levelConfig);
    }
    public override bool IsFinish()
    {
        return StimulateEntryControllerModel.Instance.IsFinish(_levelConfig);
    }
    private TableStimulateSetting _levelConfig;
    public override void SetConfig(int configId)
    {
        _levelConfig = StimulateConfigManager.Instance._stimulateSetting.Find(a => a.id == configId);
    }

    public override void CreateCell()
    {
        _normalCell = new UITabulationCellStimulate();
        _lockCell = new UITabulationCellStimulate();
        _comingsoonCell = new UITabulationCellStimulate();
    }
}