using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Asset;
using Makeover;
using MiniGame;
using UnityEngine;

public class UITabulationCellAsmr:UITabulationCell
{
    private TableMoLevel _levelConfig;
    public override int GetUnlockDecoNodeNum()
    {
        return _levelConfig.unlockNodeNum;
    }

    public override void OnClickPlayBtn()
    {
        SceneFsm.mInstance.ChangeState(StatusType.Makeover, _levelConfig,false);
    }

    public override Sprite GetIcon()
    {
        return ResourcesManager.Instance.GetSpriteVariant("MakeOverAtlas", _levelConfig.icon);
    }

    public override Dictionary<string, string> GetNeedDownloadAssets()
    {
        return ASMR.Model.Instance.GetNeedDownloadAssets(_levelConfig.levelId);
    }

    public override void SetConfig(int configId)
    {
        _levelConfig = MakeoverConfigManager.Instance.levelList.Find(a => a.id == configId);
    }

    public override bool IsUnlock()
    {
        return MakeoverModel.Instance.IsUnLock(_levelConfig);
    }

    public override bool IsFinish()
    {
        return MakeoverModel.Instance.IsFinish(_levelConfig);
    }

    public override bool NeedUpdateOnDownload(int configId)
    {
        var config = MakeoverConfigManager.Instance.levelList.Find(a => a.id == configId);
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
            EventDispatcher.Instance.DispatchEvent(EventEnum.ASMR_End_DOWNLOAD, _levelConfig);
        } );
        EventDispatcher.Instance.DispatchEvent(EventEnum.ASMR_BEGIN_DOWNLOAD, _levelConfig);
    }
    public async void DownloadAssets(int levelId, Action<bool> onFinished)
    {
        var assets = ASMR.Model.Instance.GetNeedDownloadAssets(levelId);
        if (assets == null || assets.Count == 0)
        {
            onFinished?.Invoke(true);
            return;
        }
            
        var downLoadFinish = false;
        var downloadTaskList = ASMR.Model.Instance.DownloadFiles(assets, (success) => downLoadFinish = true);
    
        while (!downLoadFinish)
        {
            ASMR.Model.Instance.UpdateProgressFromDownloadInfoList(downloadTaskList, (progress,extralInfo)=>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.ASMR_DOWNLOAD_PROGRESS, _levelConfig, progress);
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
public class UITabulationItemAsmr:UITabulationItem
{
    public override bool IsUnlock()
    {
        return MakeoverModel.Instance.IsUnLock(_levelConfig);
    }
    public override bool IsFinish()
    {
        return MakeoverModel.Instance.IsFinish(_levelConfig);
    }
    private TableMoLevel _levelConfig;
    public override void SetConfig(int configId)
    {
        _levelConfig = MakeoverConfigManager.Instance.levelList.Find(a => a.id == configId);
    }

    public override void CreateCell()
    {
        _normalCell = new UITabulationCellAsmr();
        _lockCell = new UITabulationCellAsmr();
        _comingsoonCell = new UITabulationCellAsmr();
    }
}