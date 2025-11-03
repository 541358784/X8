using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Stimulate.Configs;
using Stimulate.FSM_Stimulate;
using Stimulate.Model;
using Stimulate.View;
using UnityEngine;

namespace Stimulate.FSM_Stimulate.States
{
    public class FsmTryGetReward : FsmStateBase
    {
        private UIStimulateController _controller;
        private TableStimulateNodes _config;
        
        public override void OnEnter(params object[] param)
        {
            _controller = (UIStimulateController)param[0];
            _config = (TableStimulateNodes)param[1];
            
            var nodes = StimulateConfigManager.Instance.GetNodes(StimulateModel.Instance._config.levelId);
            foreach (var config in nodes)
            {
                if (StimulateModel.Instance.GetNodeState(config.levelId, config.id) == StimulateModel.NodeState.Finish)
                    continue;
                
                Fsm.ChangeState<FsmInitSkin>(_controller);
                return;
            }

            TryGetReward();
        }
        
        public override void OnExit()
        {
            
        }
        
        
        private void TryGetReward(bool playFlyEffect = true)
        {
            if (StimulateModel.Instance._config.rewardId == null || StimulateModel.Instance._config.rewardId.Length == 0)
                return;

            for (int i = 0; i < StimulateModel.Instance._config.rewardId.Length; i++)
            {
                if (Gameplay.UserData.Instance.IsResource(StimulateModel.Instance._config.rewardId[i]))
                {
                    Gameplay.UserData.Instance.AddRes(StimulateModel.Instance._config.rewardId[i], StimulateModel.Instance._config.rewardNum[i],
                        new GameBIManager.ItemChangeReasonArgs()
                        {
                            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.AsmrGet
                        }, false);
                }
                else
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(StimulateModel.Instance._config.rewardId[i]);
                    if (itemConfig != null)
                    {
                        var mergeItem = MergeManager.Instance.GetEmptyItem();
                        mergeItem.Id = StimulateModel.Instance._config.rewardId[i];
                        mergeItem.State = 1;
                        MergeManager.Instance.AddRewardItem(mergeItem, MergeBoardEnum.Main);
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
                if (GameBIManager.TryParseGameEventType(ftueBi, out ftueBiEvent))
                    GameBIManager.Instance.SendGameEvent(ftueBiEvent);
            }

            if (playFlyEffect)
            {
                FlyReward();
            }
            else
            {
                Fsm.ChangeState<FsmInitSkin>(_controller);
            }
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

            var levelCfg = StimulateModel.Instance._config;
            if (levelCfg.rewardId != null && levelCfg.rewardId.Length > 0)
            {
                for (int i = 0; i < levelCfg.rewardId.Length; i++)
                {
                    int index = i;
                    if (Gameplay.UserData.Instance.IsResource(levelCfg.rewardId[i]))
                    {
                        FlyGameObjectManager.Instance.FlyCurrency(
                            CurrencyGroupManager.Instance.GetCurrencyUseController(),
                            (Gameplay.UserData.ResourceId)levelCfg.rewardId[i], levelCfg.rewardNum[i], Vector2.zero, 0.8f,
                            true, true, 0.15f,
                            () =>
                            {
                                if (index == levelCfg.rewardId.Length - 1)
                                {
                                    Fsm.ChangeState<FsmInitSkin>(_controller);
                                }
                            });
                    }
                    else
                    {
                        FlyGameObjectManager.Instance.FlyObject(levelCfg.rewardId[i], Vector2.zero, endPos,
                            1.2f, 2.0f, 1f,
                            () =>
                            {
                                if (index == levelCfg.rewardId.Length - 1)
                                {
                                    Fsm.ChangeState<FsmInitSkin>(_controller);
                                }
                                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                            });
                    }
                }
            }
        }
    }
}