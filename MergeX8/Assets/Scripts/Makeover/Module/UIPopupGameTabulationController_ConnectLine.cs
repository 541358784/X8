using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectLine;
using ConnectLine.Model;
using DragonU3DSDK.Asset;
using ConnectLineSpace;
using UnityEngine;

public class UITabulationCellConnectLine:UITabulationCell
{
    private TableConnectLineLevel _levelConfig;
    public override int GetUnlockDecoNodeNum()
    {
        return _levelConfig.unlockNodeNum;
    }

    public override void OnClickPlayBtn()
    {
        SceneFsm.mInstance.ChangeState(StatusType.EnterConnectLine, _levelConfig,false);
    }

    public override Sprite GetIcon()
    {
        return ResourcesManager.Instance.GetSpriteVariant("ConnectLineAtlas", _levelConfig.icon);
    }

    public override Dictionary<string, string> GetNeedDownloadAssets()
    {
        return ConnectLineSpace.Model.Instance.GetNeedDownloadAssets(_levelConfig.levelId);
    }

    public override void SetConfig(int configId)
    {
        _levelConfig = ConnectLineConfigManager.Instance._configs.Find(a => a.id == configId);
    }

    public override bool IsUnlock()
    {
        return ConnectLineModel.Instance.IsUnLock(_levelConfig);
    }

    public override bool IsFinish()
    {
        return ConnectLineModel.Instance.IsFinish(_levelConfig);
    }

    public override bool NeedUpdateOnDownload(int configId)
    {
        var config = ConnectLineConfigManager.Instance._configs.Find(a => a.id == configId);
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
            EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECT_LINE_End_DOWNLOAD, _levelConfig);
        } );
        EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECT_LINE_BEGIN_DOWNLOAD, _levelConfig);
    }
    public async void DownloadAssets(int levelId, Action<bool> onFinished)
    {
        var assets = ConnectLineSpace.Model.Instance.GetNeedDownloadAssets(levelId);
        if (assets == null || assets.Count == 0)
        {
            onFinished?.Invoke(true);
            return;
        }
            
        var downLoadFinish = false;
        var downloadTaskList = ConnectLineSpace.Model.Instance.DownloadFiles(assets, (success) => downLoadFinish = true);
    
        while (!downLoadFinish)
        {
            ConnectLineSpace.Model.Instance.UpdateProgressFromDownloadInfoList(downloadTaskList, (progress,extralInfo)=>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECT_LINE_DOWNLOAD_PROGRESS, _levelConfig, progress);
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
public class UITabulationItemConnectLine:UITabulationItem
{
    public override bool IsUnlock()
    {
        return ConnectLineModel.Instance.IsUnLock(_levelConfig);
    }
    public override bool IsFinish()
    {
        return ConnectLineModel.Instance.IsFinish(_levelConfig);
    }
    private TableConnectLineLevel _levelConfig;
    public override void SetConfig(int configId)
    {
        _levelConfig = ConnectLineConfigManager.Instance._configs.Find(a => a.id == configId);
    }

    public override void CreateCell()
    {
        _normalCell = new UITabulationCellConnectLine();
        _lockCell = new UITabulationCellConnectLine();
        _comingsoonCell = new UITabulationCellConnectLine();
    }
}