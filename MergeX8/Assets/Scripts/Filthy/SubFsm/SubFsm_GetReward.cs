using DragonPlus;
using DragonPlus.Config.Filthy;
using DragonU3DSDK.Network.API.Protocol;
using Filthy.Model;
using Filthy.SubFsm.Base;
using Filthy.View;
using UnityEngine;

namespace Filthy.SubFsm
{
    public class SubFsm_GetReward : FsmBase
    {
        private UIFilthyController _controller;
        private FilthyNodes _config;
        
        public override void OnEnter(params object[] param)
        {
            _controller = (UIFilthyController)param[0];
            _config = (FilthyNodes)param[1];
            
            var nodes = FilthyConfigManager.Instance.FilthyNodesList.FindAll(a=>a.LevelId == FilthyModel.Instance.LevelId());
            foreach (var config in nodes)
            {
                if (FilthyModel.Instance.GetNodeState(config.LevelId, config.Id) == FilthyModel.NodeState.Finish)
                    continue;
                
                Fsm.ChangeState<SubFsm_InitSkin>(_controller);
                return;
            }

            TryGetReward();
        }
        
        public override void OnExit()
        {
            
        }
        
        
        private void TryGetReward(bool playFlyEffect = true)
        {
            if (FilthyModel.Instance._config.RewardId == null || FilthyModel.Instance._config.RewardId.Count == 0)
                return;

            for (int i = 0; i < FilthyModel.Instance._config.RewardId.Count; i++)
            {
                if (Gameplay.UserData.Instance.IsResource(FilthyModel.Instance._config.RewardId[i]))
                {
                    Gameplay.UserData.Instance.AddRes(FilthyModel.Instance._config.RewardId[i], FilthyModel.Instance._config.RewardNum[i],
                        new GameBIManager.ItemChangeReasonArgs()
                        {
                            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.AsmrGet
                        }, false);
                }
                else
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(FilthyModel.Instance._config.RewardId[i]);
                    if (itemConfig != null)
                    {
                        var mergeItem = MergeManager.Instance.GetEmptyItem();
                        mergeItem.Id = FilthyModel.Instance._config.RewardId[i];
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


            if (_config.Id == 1)
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
                Fsm.ChangeState<SubFsm_InitSkin>(_controller);
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

            var levelCfg = FilthyModel.Instance._config;
            if (levelCfg.RewardId != null && levelCfg.RewardId.Count > 0)
            {
                for (int i = 0; i < levelCfg.RewardId.Count; i++)
                {
                    int index = i;
                    if (Gameplay.UserData.Instance.IsResource(levelCfg.RewardId[i]))
                    {
                        FlyGameObjectManager.Instance.FlyCurrency(
                            CurrencyGroupManager.Instance.GetCurrencyUseController(),
                            (Gameplay.UserData.ResourceId)levelCfg.RewardId[i], levelCfg.RewardNum[i], Vector2.zero, 0.8f,
                            true, true, 0.15f,
                            () =>
                            {
                                if (index == levelCfg.RewardId.Count - 1)
                                {
                                    Fsm.ChangeState<SubFsm_InitSkin>(_controller);
                                }
                            });
                    }
                    else
                    {
                        FlyGameObjectManager.Instance.FlyObject(levelCfg.RewardId[i], Vector2.zero, endPos,
                            1.2f, 2.0f, 1f,
                            () =>
                            {
                                if (index == levelCfg.RewardId.Count - 1)
                                {
                                    Fsm.ChangeState<SubFsm_InitSkin>(_controller);
                                }
                                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                            });
                    }
                }
            }
        }
    }
}