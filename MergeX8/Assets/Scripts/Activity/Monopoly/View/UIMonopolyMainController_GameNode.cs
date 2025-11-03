using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Asset;
using UnityEngine;

public partial class UIMonopolyMainController
{
    private GameNode Game;
    private bool InitGameNodeFlag = false;
    public void InitGameNode()
    {
        if (InitGameNodeFlag)
            return;
        InitGameNodeFlag = true;
        Game = transform.Find("Root/Game").gameObject.AddComponent<GameNode>();
        Game.MainUI = this;
        Game.Init(/*Storage.ScoreMultiValue() */BetValue ,Storage.CurBlockIndex,Storage.BlockBuyState,Storage.CurBlockBuyState);
        
    }
    public void PerformLevelUp(EventMonopolyLevelUp evt)
    {
        Action<Action> performAction = async (callback) =>
        {
            callback();
        };
        PushPerformAction(performAction);
    }
    public class GameNode:MonoBehaviour
    {
        public UIMonopolyMainController MainUI;
        private List<BlockNode> BlockNodeList = new List<BlockNode>();
        private PlayerNode Player;
        private Dictionary<int, int> BlockBuyState;
        
        public void Init(int scoreMultiValue,int curBlockIndex,Dictionary<int,int> blockBuyState,bool curBlockBuyState)
        {
            BlockBuyState = blockBuyState;
            var blockList = MonopolyModel.Instance.BlockConfigList;
            for (var i = 0; i < blockList.Count; i++)
            {
                var blockConfig = blockList[i];
                var blockNode = transform.Find(i.ToString()).gameObject.AddComponent<BlockNode>();
                // blockNode.transform.SetAsFirstSibling();
                var full = MainUI.Storage.IsBlockGroupFull(blockConfig);
                blockNode.Init(blockConfig,MainUI.Storage.GetBlockBuyTimes(blockConfig.Id),full);
                if (full)
                {
                    var effect = transform.Find(blockConfig.GroupEffect);
                    effect.gameObject.SetActive(true);
                }
                blockNode.SetScoreMultiValue(scoreMultiValue);
                BlockNodeList.Add(blockNode);
            }
            
            Player = transform.Find("Player").gameObject.AddComponent<PlayerNode>();
            Player.gameObject.SetActive(true);
            var curBlock = BlockNodeList[curBlockIndex];
            Player.SetBlockIndex(curBlockIndex);
            Player.transform.position = curBlock.transform.position;
        }
        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventMonopolyUIGetBlockReward>(PerformGetBlockReward);
            EventDispatcher.Instance.AddEvent<EventMonopolyUIGetBlockScore>(PerformGetBlockScore);
            EventDispatcher.Instance.AddEvent<EventMonopolyUIMoveStep>(PerformMoveStep);
            EventDispatcher.Instance.AddEvent<EventMonopolyScoreMultiChange>(PerformScoreMultiChange);
            EventDispatcher.Instance.AddEvent<EventMonopolyUIBetChange>(PerformBetChange);
            EventDispatcher.Instance.AddEvent<EventMonopolyUIBuyBlock>(PerformBuyBlock);
        }
        
        public void PerformGetBlockReward(EventMonopolyUIGetBlockReward evt)
        {
            var rewards = evt.Reward;
            Action<Action> performAction = (callback) =>
            {
                if (UIMonopolyMainController.IsAuto)
                {
                    callback();
                }
                else
                {
                    CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                        false, animEndCall: callback);
                }
            };
            MainUI.PushPerformAction(performAction);
        }
        public void PerformGetBlockScore(EventMonopolyUIGetBlockScore evt)
        {
            Action<Action> performAction = (callback) =>
            {
                var position = BlockNodeList[evt.BlockIndex].transform.position;
                if (!GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyGetScore, null))
                {
                    MainUI.FlyCarrot(position,evt.Score,callback);
                }
                else
                {
                    MainUI.FlyCarrot(position,evt.Score);
                    Action<BaseEvent> guideFinishCallback = null;
                    guideFinishCallback = (evt) =>
                    {
                        if (evt.datas.Length < 1)
                            return;
                        var config = evt.datas[0] as TableGuide;
                        if (config == null)
                            return;
                        if (config.triggerPosition == (int) GuideTriggerPosition.MonopolyGetScore)
                        {
                            EventDispatcher.Instance.RemoveEventListener(EventEnum.GuideFinish, guideFinishCallback);
                            callback();
                        }
                    };
                    EventDispatcher.Instance.AddEventListener(EventEnum.GuideFinish,guideFinishCallback);
                }
            };
            MainUI.PushPerformAction(performAction);
        }
        public void PerformMoveStep(EventMonopolyUIMoveStep evt)
        {
            var startIndex = evt.StartIndex;
            var endIndex = evt.StartIndex + evt.MoveIndex;
            // if (endIndex > BlockNodeList.Count - 1)
            //     endIndex = BlockNodeList.Count - 1;
            Action<Action> performAction = async (callback) =>
            {
                for (var i = startIndex; i < endIndex; i++)
                {
                    var targetIndex = i + 1;
                    targetIndex %= BlockNodeList.Count;
                    var targetBlock = BlockNodeList[targetIndex];
                    var task = new TaskCompletionSource<bool>();
                    AudioManager.Instance.PlaySound(114);
                    Player.transform.DOJump(targetBlock.transform.position, MonopolyModel.Instance.GlobalConfig.JumpPower, 1, 0.3f).OnComplete(() =>
                    {
                        task.SetResult(true);
                    });
                    await task.Task;
                    await XUtility.WaitSeconds(0.2f);
                }
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }

        public void PerformScoreMultiChange(EventMonopolyScoreMultiChange evt)
        {
            Action<Action> performAction = (callback) =>
            {
                foreach (var block in BlockNodeList)
                {
                    block.SetScoreMultiValue(evt.NewValue);
                }
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }
        public void PerformBetChange(EventMonopolyUIBetChange evt)
        {
            Action<Action> performAction = (callback) =>
            {
                foreach (var block in BlockNodeList)
                {
                    block.SetScoreMultiValue(evt.BetValue);
                }
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }

        public void PerformBuyBlock(EventMonopolyUIBuyBlock evt)
        {
            var nodeGroup = BlockNodeList.FindAll((a) => evt.BlockConfig.GroupMember.Contains(a.BlockConfig.Id));
            if (nodeGroup.Count == 0)
                return;
            var full = MainUI.Storage.IsBlockGroupFull(evt.BlockConfig);
            Action<Action> performAction = async (callback) =>
            {
                foreach (var node in nodeGroup)
                {
                    if (node.BlockConfig == evt.BlockConfig)
                    {
                        node.SetBuyTimes(evt.BuyTimes);
                    }
                }
                AudioManager.Instance.PlaySound(118);
                callback();
                await XUtility.WaitSeconds(0.3f);
                var triggerFull = false;
                foreach (var node in nodeGroup)
                {
                    if (node.SetGroupFull(full))
                    {
                        triggerFull = true;
                    }
                }
                if (triggerFull)
                {
                    var tip = MainUI.transform.Find("Root/Tip");
                    tip.DOKill();
                    tip.gameObject.SetActive(false);
                    tip.gameObject.SetActive(true);
                    DOVirtual.DelayedCall(2f, () => tip.gameObject.SetActive(false)).SetTarget(tip);
                    var effect = transform.Find(nodeGroup[0].BlockConfig.GroupEffect);
                    effect.gameObject.SetActive(true);
                    AudioManager.Instance.PlaySound(118);
                }
            };
            MainUI.PushPerformAction(performAction);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventMonopolyUIGetBlockReward>(PerformGetBlockReward);
            EventDispatcher.Instance.RemoveEvent<EventMonopolyUIGetBlockScore>(PerformGetBlockScore);
            EventDispatcher.Instance.RemoveEvent<EventMonopolyUIMoveStep>(PerformMoveStep);
            EventDispatcher.Instance.RemoveEvent<EventMonopolyScoreMultiChange>(PerformScoreMultiChange);
            EventDispatcher.Instance.RemoveEvent<EventMonopolyUIBuyBlock>(PerformBuyBlock);
            EventDispatcher.Instance.RemoveEvent<EventMonopolyUIBetChange>(PerformBetChange);
        }
    }
}