using System;
using Decoration.DynamicMap;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Psychology.Model;
using Psychology.System;
using UnityEngine;
using AudioManager = DragonPlus.AudioManager;

namespace Psychology.Fsm
{
    public class SceneFsmEnterPsychology : IFsmState
    {
        public StatusType Type
        {
            get { return StatusType.EnterPsychology; }
        }

        private TablePsychology _config;

        private PsychologySystem _system;
        
        private bool _isFinish = false;
        public void Enter(params object[] objs)
        {
            Input.multiTouchEnabled = false;
            
            UILoadingTransitionController.Hide(() =>
            {
            });
            _config = (TablePsychology)objs[0];

            _system = new PsychologySystem();
            _system.Enter();
            
            _isFinish = PsychologyModel.Instance.IsFinish(_config);
            
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameRoomsortLevelstart,
                data1:_config.id.ToString());
            
            UIManager.Instance.SetCanvasGroupAlpha(false, false);
            var guideWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGuidePortrait);
            if (guideWindow)
            {
                guideWindow.canvasGroup.alpha = 1f;
            }
            AudioManager.Instance.PlayMusic("bgm_blueprint", true);
            
            DynamicMapManager.Instance.PauseLogic = true;
            
            // AudioManager.Instance.StopAllMusic();

            PlayerManager.Instance.HidePlayer();

#if DEVELOPMENT_BUILD || DEBUG
            if(PsychologyModel.Instance.debugLevelId > 0)
                _config.id = PsychologyModel.Instance.debugLevelId;
#endif
            
            PsychologyModel.Instance.InitModel(_config);
            // UIManager.Instance.OpenUI(UINameConst.UIPsychologyMain);
            UIBlueBlockMainController.Open(_config);
            
            // GameBIManager.Instance.SendGameEvent( BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigamePsychologyChoose,data1:_config.id.ToString(), "0");
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
            
            UIManager.Instance.SetCanvasGroupAlpha(true, false);
            Resources.UnloadUnusedAssets();
            
            // UIManager.Instance.CloseUI(UINameConst.UIPsychologyMain, true);
            UIManager.Instance.CloseUI(UINameConst.UIBlueBlockMain, true);
            
            UIManager.Instance.EnableGraphicRaycaster(true);
            if ( _config.id == 1 && !_isFinish && PsychologyModel.Instance.IsFinish(_config))
            {
                AddReward(false);
            }
            else
            {
                XUtility.WaitSeconds(0.1f, () =>
                {
                    AudioManager.Instance.PlayMusic(1, true);
                });
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
        
        if(!PsychologyModel.Instance.IsFinish(_config))
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