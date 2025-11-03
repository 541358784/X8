using System;
using Decoration;
using Decoration.DynamicMap;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using Makeover;
using UnityEngine;
using Utils = Makeover.Utils;

public class SceneFsmMakeover : IFsmState
{
    public StatusType Type => StatusType.Makeover;

    private MakeoverSystem makeoverSystem;

    private TableMoLevel _curLevelConfig;

    private bool _isFinish = false;
    public void Enter(params object[] objs)
    {
        _curLevelConfig = (TableMoLevel)objs[0];
        Utils.makeOverLevel = _curLevelConfig.id;

        DynamicMapManager.Instance.PauseLogic = true;
        
        makeoverSystem = new MakeoverSystem();
        makeoverSystem.Enter();

        _isFinish = MakeoverModel.Instance.IsFinish(_curLevelConfig);
        
        AudioManager.Instance.StopAllMusic();
        EnterFinish();

        int index = MakeoverModel.Instance.GetBodyStep(_curLevelConfig);
        
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrLevelstart,
            data1:_curLevelConfig.id.ToString());
        ASMR.Model.Instance.LoadLevel(_curLevelConfig);
        
        if (_curLevelConfig.newLevel)
        {
            Input.multiTouchEnabled = true;
            CameraManager.MainCamera.gameObject.SetActive(false);
        }
        
        StorageMakeOver storageMakeOver = StorageManager.Instance.GetStorage<StorageHome>().MakeOver;
        storageMakeOver.IsFinish = true;
        
        UILoadingTransitionController.Hide(() =>
        {
            if(_curLevelConfig !=null && !_curLevelConfig.newLevel)
                UIManager.Instance.OpenUI(UINameConst.UIGameMain, _curLevelConfig);
        });
        
        var hideObj = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find("Surface/Movie_Ship/ripple/Spine GameObject (kelp)").gameObject;
        hideObj.gameObject.SetActive(false);
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventAsmrEnter);

        PlayerManager.Instance.HidePlayer();
    }

    public void EnterFinish()
    {
        UIManager.Instance.SetCanvasGroupAlpha(false, false);
        AudioManager.Instance.PlayMusic("sfx_ASMR_bg", true);
    }

    public void Update(float deltaTime)
    {
        makeoverSystem.Update(deltaTime);
    }
    public void LateUpdate(float deltaTime)
    {
        
    }

    public void Exit()
    {
        makeoverSystem.Exit();
        makeoverSystem = null;

        DynamicMapManager.Instance.PauseLogic = false;
        
        ASMR.Model.Instance.ExitGame();
        Utils.DestroyRoot();

        AudioManager.Instance.PlayMusic(1, true);
        UIManager.Instance.SetCanvasGroupAlpha(true, false);
        Resources.UnloadUnusedAssets();
        UIManager.Instance.CloseUI(UINameConst.UIGameMain, true);
        //DecoSceneRoot.Instance.mSceneCamera.gameObject.SetActive(true);

        if (_curLevelConfig.newLevel)
        {
            CameraManager.MainCamera.gameObject.SetActive(true);
        }
        
        if (!GuideSubSystem.Instance.isFinished(101))
        {
            StorySubSystem.Instance.Trigger(StoryTrigger.Asmr, _curLevelConfig.id.ToString(), b =>
            {
                AddReward();
                if(_curLevelConfig.id == 1)
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
            });
        }
        else
        {
            AddReward();
        }

        string bi = "GAME_EVENT_FTUE_COMPLETE_ASMR"+_curLevelConfig.id;
        BiEventAdventureIslandMerge.Types.GameEventType biEvent;
        if(GameBIManager.TryParseGameEventType(bi, out biEvent))
            GameBIManager.Instance.SendGameEvent(biEvent);

        
        EventDispatcher.Instance.DispatchEvent(EventEnum.ASMR_REFRESH_REDPOINT);
    }

    private void AddReward()
    {
        if(_isFinish)
            return;
        
        if(!MakeoverModel.Instance.IsFinish(_curLevelConfig))
            return;
            
        if (_curLevelConfig.rewardID == null || _curLevelConfig.rewardID.Length == 0)
            return;
        
        for (int i = 0; i < _curLevelConfig.rewardID.Length; i++)
        {
            if (UserData.Instance.IsResource(_curLevelConfig.rewardID[i]))
            {
                UserData.Instance.AddRes(_curLevelConfig.rewardID[i], _curLevelConfig.rewardCnt[i],
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.AsmrGet
                    }, false);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(_curLevelConfig.rewardID[i]);
                if (itemConfig != null)
                {
                    var mergeItem = MergeManager.Instance.GetEmptyItem();
                    mergeItem.Id = _curLevelConfig.rewardID[i];
                    mergeItem.State = 1;
                    MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType
                            .MergeChangeReasonEasterReward,
                        isChange = true,
                    });
                }
            }
        }
        
        
        if (_curLevelConfig.id == 1)
        {
            string ftueBi = "GAME_EVENT_FTUE_NEW_7";
            BiEventAdventureIslandMerge.Types.GameEventType ftueBiEvent;
            if(GameBIManager.TryParseGameEventType(ftueBi, out ftueBiEvent))
                GameBIManager.Instance.SendGameEvent(ftueBiEvent);
        }

        FlyReward();
    }
    public void FlyReward()
    {
        Vector3 endPos = Vector3.zero;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            endPos = MergeMainController.Instance.rewardBtnPos;
        }
        else
        {
            endPos = UIHomeMainController.mainController.MainPlayTransform.position;
        }

        var levelCfg = _curLevelConfig;
        if (levelCfg.rewardID != null && levelCfg.rewardID.Length > 0)
        {
            for (int i = 0; i < levelCfg.rewardID.Length; i++)
            {
                if (UserData.Instance.IsResource(levelCfg.rewardID[i]))
                {
                    FlyGameObjectManager.Instance.FlyCurrency(
                        CurrencyGroupManager.Instance.GetCurrencyUseController(),
                        (UserData.ResourceId) levelCfg.rewardID[i], levelCfg.rewardCnt[i], Vector2.zero, 0.8f,
                        true, true, 0.15f,
                        () => { });
                }
                else
                {
                    FlyGameObjectManager.Instance.FlyObject(levelCfg.rewardID[i], Vector2.zero, endPos,
                        1.2f, 2.0f, 1f,
                        () => { EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH); });
                }
            }
        }
    }
}