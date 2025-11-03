using System;
using Decoration;
using Decoration.DynamicMap;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using FishEatFishSpace;
using DragonPlus.Config.FishEatFish;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;
using Utils = FishEatFishSpace.Utils;

public class SceneFsmFishEatFish : IFsmState
{
    public StatusType Type => StatusType.FishEatFish;

    // private MakeoverSystem makeoverSystem;

    private FishEatFishLevel _curLevelConfig;

    private bool _isFinish = false;
    public void Enter(params object[] objs)
    {
        _curLevelConfig = (FishEatFishLevel)objs[0];
        var isFirstTimePlay = (bool) objs[1];

        DynamicMapManager.Instance.PauseLogic = true;
        
        // makeoverSystem = new MakeoverSystem();
        // makeoverSystem.Enter();

        _isFinish = FishEatFishEntryControllerModel.Instance.IsFinish(_curLevelConfig);
        
        AudioManager.Instance.StopAllMusic();
        EnterFinish();
        

        StorageFishEatFish storageFishEatFish = StorageManager.Instance.GetStorage<StorageHome>().FishEatFish;
        
        UILoadingTransitionController.Hide(() =>
        {
            // if (_curLevelConfig != null)
            // {
            //     UIManager.Instance.EnableGraphicRaycaster(false);
            //     var guideWindow = UIManager.Instance.GetOpenUI(UINameConst.UIGuidePortrait);
            //     if (guideWindow)
            //     {
            //         guideWindow.GetComponent<GraphicRaycaster>().enabled = true;   
            //     }
            //     UIManager.Instance.OpenUI(UINameConst.UIFishEatFishMain);
            //     FishEatFish.Model.Instance.LoadLevel(_curLevelConfig,isFirstTimePlay);
            // }
        });
        
        var hideObj = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find("Surface/Movie_Ship/ripple/Spine GameObject (kelp)").gameObject;
        hideObj.gameObject.SetActive(false);

        PlayerManager.Instance.HidePlayer();
        if (_curLevelConfig != null)
        {
            UIManager.Instance.EnableGraphicRaycaster(false);
            var guideWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGuidePortrait);
            if (guideWindow)
            {
                guideWindow.GetComponent<GraphicRaycaster>().enabled = true;
            }
            FishEatFishSpace.Model.Instance.LoadLevel(_curLevelConfig, isFirstTimePlay);
        }
    }

    public void EnterFinish()
    {
        UIManager.Instance.SetCanvasGroupAlpha(false, false);
        var guideWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGuidePortrait);
        if (guideWindow)
        {
            guideWindow.canvasGroup.alpha = 1f;
        }
        AudioManager.Instance.PlayMusic("bgm_dig", true);
    }

    public void Update(float deltaTime)
    {
        // makeoverSystem.Update(deltaTime);
    }
    public void LateUpdate(float deltaTime)
    {
        
    }

    public void Exit()
    {
        // makeoverSystem.Exit();
        // makeoverSystem = null;

        DynamicMapManager.Instance.PauseLogic = false;
        
        // FishEatFish.Model.Instance.ExitGame();
        Utils.DestoryRoot();
        XUtility.WaitSeconds(0.1f, () =>
        {
            AudioManager.Instance.PlayMusic(1, true);
        });
        UIManager.Instance.SetCanvasGroupAlpha(true, false);
        Resources.UnloadUnusedAssets();
        UIManager.Instance.CloseUI(UINameConst.UIFishEatFishMain, true);
        UIManager.Instance.EnableGraphicRaycaster(true);
        //DecoSceneRoot.Instance.mSceneCamera.gameObject.SetActive(true);

        if (!GuideSubSystem.Instance.isFinished(101))
        {
            StorySubSystem.Instance.Trigger(StoryTrigger.Asmr, _curLevelConfig.id.ToString(), b =>
            {
                AddReward();
                // if(_curLevelConfig.id == 1)
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
            });
        }
        else
        {
            AddReward();
        }


        EventDispatcher.Instance.DispatchEvent(EventEnum.FISH_EAT_FISH_REFRESH_REDPOINT);
    }

    private void AddReward()
    {
        if(_isFinish)
            return;
        
        if(!FishEatFishEntryControllerModel.Instance.IsFinish(_curLevelConfig))
            return;
            
        if (_curLevelConfig.rewardID == null || _curLevelConfig.rewardID.Length == 0)
            return;
        
        for (int i = 0; i < _curLevelConfig.rewardID.Length; i++)
        {
            if (Gameplay.UserData.Instance.IsResource(_curLevelConfig.rewardID[i]))
            {
                Gameplay.UserData.Instance.AddRes(_curLevelConfig.rewardID[i], _curLevelConfig.rewardCnt[i],
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MinigameFishGet
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
                if (Gameplay.UserData.Instance.IsResource(levelCfg.rewardID[i]))
                {
                    FlyGameObjectManager.Instance.FlyCurrency(
                        CurrencyGroupManager.Instance.GetCurrencyUseController(),
                        (Gameplay.UserData.ResourceId) levelCfg.rewardID[i], levelCfg.rewardCnt[i], Vector2.zero, 0.8f,
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