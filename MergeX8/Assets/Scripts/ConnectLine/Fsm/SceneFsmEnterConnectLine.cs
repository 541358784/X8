using System;
using ConnectLine.Logic;
using ConnectLine.Model;
using ConnectLine.System;
using Decoration;
using Decoration.DynamicMap;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Makeover;
using OneLine;
using OnePath.Model;
using OnePath.System;
using OnePath.View;
using OnePathSpace;
using TMatch;
using UnityEngine;
using AudioManager = DragonPlus.AudioManager;

namespace ConnectLine.Fsm
{
    public class SceneFsmEnterConnectLine : IFsmState
    {
        public StatusType Type
        {
            get { return StatusType.EnterConnectLine; }
        }

        private TableConnectLineLevel _config;

        private ConnectLineSystem _system;
        
        private bool _isFinish = false;
        public void Enter(params object[] objs)
        {
            Input.multiTouchEnabled = false;
            
            UILoadingTransitionController.Hide(() =>
            {
            });
            _config = (TableConnectLineLevel)objs[0];

            _system = new ConnectLineSystem();
            _system.Enter();
            
            _isFinish = ConnectLineModel.Instance.IsFinish(_config);
            
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameConnectLineLevelstart,
                data1:_config.id.ToString());
            
            UIManager.Instance.SetCanvasGroupAlpha(false, false);
            var guideWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGuidePortrait);
            if (guideWindow)
            {
                guideWindow.canvasGroup.alpha = 1f;
            }
            AudioManager.Instance.PlayMusic("bgm_dig", true);
            
            DynamicMapManager.Instance.PauseLogic = true;
            
            AudioManager.Instance.StopAllMusic();

            PlayerManager.Instance.HidePlayer();

#if DEVELOPMENT_BUILD || DEBUG
            if(ConnectLineModel.Instance.debugLevelId > 0)
                _config.levelId = ConnectLineModel.Instance.debugLevelId;
#endif
            
            ConnectLineModel.Instance.InitModel(_config);
            UIManager.Instance.OpenUI(UINameConst.UIConnectLineMain);
        }
        
        public void Update(float deltaTime)
        {
        }

        public void LateUpdate(float deltaTime)
        {
        }

        public void Exit()
        {
            _system.Exit();
            _system = null;
            
            DynamicMapManager.Instance.PauseLogic = false;
        
            PathLineManager.Instance.Release();
            XUtility.WaitSeconds(0.1f, () =>
            {
                AudioManager.Instance.PlayMusic(1, true);
            });
            UIManager.Instance.SetCanvasGroupAlpha(true, false);
            Resources.UnloadUnusedAssets();
            
            UIManager.Instance.CloseUI(UINameConst.UIConnectLineMain, true);
            
            UIManager.Instance.EnableGraphicRaycaster(true);
            if ( _config.id == 1 && !_isFinish && ConnectLineModel.Instance.IsFinish(_config))
            {
                AddReward(false);
            }
            else
            {
                if (!GuideSubSystem.Instance.isFinished(101))
                {
                    StorySubSystem.Instance.Trigger(StoryTrigger.Asmr, "1", b =>
                    {
                        AddReward();
                        // if(_config.id == 1)
                        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
                    });
                }
                else
                {
                    AddReward();
                }   
                EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECT_LINE_REFRESH_REDPOINT);
            }
        }

        private void AddReward(bool playFlyEffect = true)
    {
        if(_isFinish)
            return;
        
        if(!ConnectLineModel.Instance.IsFinish(_config))
            return;
            
        if(!Makeover.Utils.IsUseNewMiniGame())
            return;
        
        if (_config.rewardID == null || _config.rewardID.Length == 0)
            return;
        
        for (int i = 0; i < _config.rewardID.Length; i++)
        {
            if (Gameplay.UserData.Instance.IsResource(_config.rewardID[i]))
            {
                Gameplay.UserData.Instance.AddRes(_config.rewardID[i], _config.rewardCnt[i],
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MinigameConnectlineGet
                    }, false);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(_config.rewardID[i]);
                if (itemConfig != null)
                {
                    var mergeItem = MergeManager.Instance.GetEmptyItem();
                    mergeItem.Id = _config.rewardID[i];
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
        
        
        if (_config.id == 1)
        {
            string ftueBi = "GAME_EVENT_FTUE_NEW_7";
            BiEventAdventureIslandMerge.Types.GameEventType ftueBiEvent;
            if(GameBIManager.TryParseGameEventType(ftueBi, out ftueBiEvent))
                GameBIManager.Instance.SendGameEvent(ftueBiEvent);
        }
        if (playFlyEffect)
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

        var levelCfg = _config;
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
}