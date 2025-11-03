using System;
using System.Collections.Generic;
using Decoration;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;


public enum StoryTrigger
{
    //购买挂点，扣除装修币后
    BuyNodeSuccess=1,
    //完成特定挂点装修【播放完装修动画后）
    BuyNodeShowSuccess=2,
    //解锁新区域
    UnLockNewArea = 3,
    //特殊活动开启
    OpenActivity = 4,
    //首次进入merge界面
    FirstEnterMerge=5,
    //首次解锁特定merge材料
    FirstGetNetMergeItem=6,
    //首次合成特定merge材料
    FirstMergeItem=7,
    //剧情电影结束
    StoryMovieEnd = 8,
    //升级
    LevelUp = 9,
    //ASMR
    Asmr = 10,
    
    //关闭ui
    CloseUI = 11,
    
    //获得挂点元素
    GetDecoItem = 20,
    //挂点元素替换完
    DecoItemSuccess = 21,
    
    EnterMap = 22,
}

public class StorySubSystem : GlobalSystem<StorySubSystem>
{
    private bool _isShowing = false;
    private Action _onDialogExit;
    private Action<bool> _onGuideTriggered;
    private TableStory _currentStoryConfig;

    public bool IsShowing
    {
        get => _isShowing;
    }

    public bool IsStoryFinished(StoryTrigger position, string param)
    {
        var config = GlobalConfigManager.Instance.GetTableStory((int)position, param);
        if (config == null)
            return false;
        
        return IsStoryFinished(config.id);
    }

    /// <param name="onStoryFinish">两个参数，第一个是否触发了剧情，第二个是否触发了教程</param>
    /// <returns>是否触发了剧情</returns>
    public bool Trigger(StoryTrigger position, string param = null, System.Action<bool> onStoryFinish = null,
        System.Action<bool> onGuideTriggered = null)
    {
        var config = GlobalConfigManager.Instance.GetTableStory((int)position, param);

        if (config == null)
        {
            onStoryFinish?.Invoke(false);
            onGuideTriggered?.Invoke(false);
            return false;
        }

        if (!config.repeatTrigger && IsStoryFinished(config.id))
        {
            onStoryFinish?.Invoke(false);
            onGuideTriggered?.Invoke(false);
            return false;
        }

        if (config.immediatelySave)
        {
            var finishedStory = StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog;
            if (finishedStory != null)
            {
                if (!finishedStory.Contains(config.id))
                {
                    finishedStory.Add(config.id);
                }
            }
        }
        
        return startDialog(config, () => onStoryFinish?.Invoke(true),
            guideTriggered => onGuideTriggered?.Invoke(guideTriggered));
    }

    private void sendBI()
    {
        try
        {
            if (!string.IsNullOrEmpty(_currentStoryConfig.bi))
            {
                BiEventAdventureIslandMerge.Types.GameEventType biEvent;
                GameBIManager.TryParseGameEventType(_currentStoryConfig.bi, out biEvent);
                GameBIManager.Instance.SendGameEvent(biEvent);
            }
        }
        catch (System.Exception e)
        {
            DebugUtil.LogError("Guide BI Exception:" + e.Message);
        }
    }

    private bool startDialog(TableStory config, Action onDialogExit, Action<bool> onGuideTriggered)
    {
        _onDialogExit = onDialogExit;
        _onGuideTriggered = onGuideTriggered;

        _currentStoryConfig = config;
        _isShowing = true;

        UIManager.Instance.SetCanvasGroupAlpha(false, true);
        
        UIStoryController.StartStory(config);

        sendBI();

        return true;
  
    }

    public bool IsStoryFinished(int storyId)
    {
        var finishedDialog = StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog;
        if (finishedDialog != null)
        {
            if (finishedDialog.Contains(storyId))
            {
                return true;
            }
        }

        return false;
    }

    public void ExitStory()
    {
        try
        {
            var finishedStory = StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog;
            if (finishedStory != null)
            {
                if (!finishedStory.Contains(_currentStoryConfig.id))
                {
                    finishedStory.Add(_currentStoryConfig.id);
                }
            }

            _isShowing = false;
            
            UIStoryController.ExitStory(() =>
            {
                if(!_currentStoryConfig.hideUI)
                    UIManager.Instance.SetCanvasGroupAlpha(true, true);
                else
                {
                    UIManager.Instance.SetCanvasGroupAlpha(false, false);
                }
                
                // 防止exitdialog 调用 start dialog，又把 _onDialogExit置空
                var onExit = _onDialogExit;
                _onDialogExit = null;

                var areaFinish = DecoManager.Instance.TriggerAreaUnlock(_currentStoryConfig.id);
                UIStoryController.AreaFinished = areaFinish;


                //教程-剧情结束触发点
                var guideTriggerd = GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, _currentStoryConfig.id.ToString());
                _onGuideTriggered?.Invoke(guideTriggerd);
                if (_onGuideTriggered == null)
                {
                    //ExpModel.Instance.CheckLevelup();
                    //ExperenceModel.Instance.LevelUp();
                }

                if (!StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.StoryEnd, _currentStoryConfig.id.ToString(),
                        b =>{}, b =>
                        {
                            // 需要最后调用
                            if (b)
                                onExit?.Invoke();
                        }))
                {
                    // 需要最后调用
                    onExit?.Invoke();
                }
            });
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    public void TriggerStoryEndGuide()
    {
        var finishedStory = StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog;
        foreach (var storyId in finishedStory)
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, storyId.ToString());
        }
    }
    
    public void DEBUG_StartStory(int storyId)
    {
        var config = GlobalConfigManager.Instance.GetTableStory(storyId);

        if (config != null && storyId > 0)
            startDialog(config, null, null);
    }
}