using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnePath;
using DragonU3DSDK.Asset;
using OnePathSpace;
using Stimulate.Configs;
using UnityEngine;

public class UITabulationCellOnePath:UITabulationCell
{
    private TableOnePathLevel _levelConfig;
    public override int GetUnlockDecoNodeNum()
    {
        return _levelConfig.unlockNodeNum;
    }

    public override void OnClickPlayBtn()
    {
        SceneFsm.mInstance.ChangeState(StatusType.EnterOnePath, _levelConfig,false);
    } 

    public override Sprite GetIcon()
    {
        return ResourcesManager.Instance.GetSpriteVariant("OnePathAtlas", _levelConfig.icon);
    }

    public override Dictionary<string, string> GetNeedDownloadAssets()
    {
        return OnePathSpace.Model.Instance.GetNeedDownloadAssets(_levelConfig.levelId);
    }

    public override void SetConfig(int configId)
    {
        _levelConfig = OnePathConfigManager.Instance._configs.Find(a => a.id == configId);
    }

    public override bool IsUnlock()
    {
        return OnePathEntryControllerModel.Instance.IsUnLock(_levelConfig);
    }

    public override bool IsFinish()
    {
        return OnePathEntryControllerModel.Instance.IsFinish(_levelConfig);
    }

    public override bool NeedUpdateOnDownload(int configId)
    {
        var config = OnePathConfigManager.Instance._configs.Find(a => a.id == configId);
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
            EventDispatcher.Instance.DispatchEvent(EventEnum.ONE_PATH_End_DOWNLOAD, _levelConfig);
        } );
        EventDispatcher.Instance.DispatchEvent(EventEnum.ONE_PATH_BEGIN_DOWNLOAD, _levelConfig);
    }
    public async void DownloadAssets(int levelId, Action<bool> onFinished)
    {
        var assets = OnePathSpace.Model.Instance.GetNeedDownloadAssets(levelId);
        if (assets == null || assets.Count == 0)
        {
            onFinished?.Invoke(true);
            return;
        }
            
        var downLoadFinish = false;
        var downloadTaskList = OnePathSpace.Model.Instance.DownloadFiles(assets, (success) => downLoadFinish = true);
    
        while (!downLoadFinish)
        {
            OnePathSpace.Model.Instance.UpdateProgressFromDownloadInfoList(downloadTaskList, (progress,extralInfo)=>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.ONE_PATH_DOWNLOAD_PROGRESS, _levelConfig, progress);
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
public class UITabulationItemOnePath:UITabulationItem
{
    public override bool IsUnlock()
    {
        return OnePathEntryControllerModel.Instance.IsUnLock(_levelConfig);
    }
    public override bool IsFinish()
    {
        return OnePathEntryControllerModel.Instance.IsFinish(_levelConfig);
    }
    private TableOnePathLevel _levelConfig;
    public override void SetConfig(int configId)
    {
        _levelConfig = OnePathConfigManager.Instance._configs.Find(a => a.id == configId);
    }

    public override void CreateCell()
    {
        _normalCell = new UITabulationCellOnePath();
        _lockCell = new UITabulationCellOnePath();
        _comingsoonCell = new UITabulationCellOnePath();
    }
}