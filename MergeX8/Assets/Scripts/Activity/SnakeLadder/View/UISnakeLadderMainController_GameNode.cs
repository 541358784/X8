using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Asset;
using UnityEngine;

public partial class UISnakeLadderMainController
{
    private GameNode Game;
    private bool InitGameNodeFlag = false;
    public void InitGameNode()
    {
        if (InitGameNodeFlag)
            return;
        InitGameNodeFlag = true;
        var curLevel = Storage.GetCurLevel();
        var asset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/SnakeLadder/"+curLevel.AssetName);
        Game = Instantiate(asset,transform.Find("Root/GameRoot")).AddComponent<GameNode>();
        Game.MainUI = this;
        Game.Init(curLevel,Storage.ScoreMultiValue,Storage.CurBlockIndex);
    }
    public void PerformLevelUp(EventSnakeLadderLevelUp evt)
    {
        var scoreMultiValue = Storage.ScoreMultiValue;
        var index = Storage.CurBlockIndex;
        Action<Action> performAction = async (callback) =>
        {
            var lastGame = Game;
            var lastGameRect = lastGame.transform as RectTransform;
            lastGameRect.DOAnchorPosX(lastGameRect.rect.width + 100, 1f).OnComplete(() =>
            {
                Destroy(lastGame.gameObject);
            });
            var curLevel = evt.LevelConfig;
            var asset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/SnakeLadder/"+curLevel.AssetName);
            Game = Instantiate(asset,transform.Find("Root/GameRoot")).AddComponent<GameNode>();
            Game.MainUI = this;
            Game.Init(curLevel,scoreMultiValue,index);
            var newGameRect = Game.transform as RectTransform;
            newGameRect.anchoredPosition = new Vector2(-newGameRect.rect.width - 100, 0);
            newGameRect.DOAnchorPosX(0, 1f).OnComplete(() =>
            {
                callback(); 
            });
        };
        PushPerformAction(performAction);
    }
    public class GameNode:MonoBehaviour
    {
        private SnakeLadderLevelConfig CurLevel;
        public UISnakeLadderMainController MainUI;
        private List<BlockNode> BlockNodeList = new List<BlockNode>();
        private PlayerNode Player;
        private Dictionary<int, Animator> SnakeAnimDic = new Dictionary<int, Animator>();
        private Dictionary<int, Animator> LadderAnimDic = new Dictionary<int, Animator>();
        public void Init(SnakeLadderLevelConfig level,int scoreMultiValue,int curBlockIndex)
        {
            CurLevel = level;
            var blockList = CurLevel.GetBlockConfigList();
            var snakeTailList = new List<int>();
            var ladderTopList = new List<int>();
            for (var i = 0; i < blockList.Count; i++)
            {
                var blockConfig = blockList[i];
                var blockNode = transform.Find("Lattice/" + i).gameObject.AddComponent<BlockNode>();
                blockNode.transform.SetAsFirstSibling();
                var blockType = (SnakeLadderBlockType) blockConfig.BlockType;
                blockNode.Init(blockConfig);
                blockNode.SetScoreMultiValue(scoreMultiValue);
                BlockNodeList.Add(blockNode);
                if (blockType == SnakeLadderBlockType.Snake)
                {
                    snakeTailList.Add(i + blockConfig.MoveStep);
                    var trans = transform.Find("Ladder/Snake" + i);
                    if (trans != null)
                    {
                        SnakeAnimDic.Add(i,transform.Find("Ladder/Snake"+i).GetComponent<Animator>());
                    }
                    else
                    {
                        SnakeAnimDic.Add(i, null);
                        Debug.LogError("找不到Ladder/Snake" + i);
                    }
                }
                else if (blockType == SnakeLadderBlockType.Ladder)
                {
                    ladderTopList.Add(i + blockConfig.MoveStep);
                    var trans = transform.Find("Ladder/Ladder" + i+"_1");
                    if (trans != null)
                    {
                        LadderAnimDic.Add(i,transform.Find("Ladder/Ladder"+i+"_1").GetComponent<Animator>());
                    }
                    else
                    {
                        LadderAnimDic.Add(i, null);
                        Debug.LogError("找不到Ladder/Ladder" + i+"_1");
                    }
                }
            }

            for (var i = 0; i < snakeTailList.Count; i++)
            {
                BlockNodeList[snakeTailList[i]].SetSnakeTail();
            }
            for (var i = 0; i < ladderTopList.Count; i++)
            {
                BlockNodeList[ladderTopList[i]].SetLadderTop();
            }

            if (transform.Find("Player").GetComponent<Animator>() == null)
            {
                var animator = transform.Find("Player").gameObject.AddComponent<Animator>();
                animator.runtimeAnimatorController = ResourcesManager.Instance.LoadResource<RuntimeAnimatorController>("Prefabs/Activity/SnakeLadder/Player");
            }
            Player = transform.Find("Player/Player").gameObject.AddComponent<PlayerNode>();
            var curBlock = BlockNodeList[curBlockIndex];
            Player.SetBlockIndex(curBlockIndex);
            Player.transform.position = curBlock.transform.position;
            if (transform.Find("Lattice").GetComponent<Animator>() == null)
            {
                var animator = transform.Find("Lattice").gameObject.AddComponent<Animator>();
                animator.runtimeAnimatorController = ResourcesManager.Instance.LoadResource<RuntimeAnimatorController>("Prefabs/Activity/SnakeLadder/Lattice");
            }
        }
        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventSnakeLadderUIGetBlockReward>(PerformGetBlockReward);
            EventDispatcher.Instance.AddEvent<EventSnakeLadderUIGetBlockScore>(PerformGetBlockScore);
            EventDispatcher.Instance.AddEvent<EventSnakeLadderUIMoveSnake>(PerformMoveSnake);
            EventDispatcher.Instance.AddEvent<EventSnakeLadderUIMoveLadder>(PerformMoveLadder);
            EventDispatcher.Instance.AddEvent<EventSnakeLadderUIMoveStep>(PerformMoveStep);
            EventDispatcher.Instance.AddEvent<EventSnakeLadderScoreMultiChange>(PerformScoreMultiChange);
        }
        
        public void PerformGetBlockReward(EventSnakeLadderUIGetBlockReward evt)
        {
            var rewards = evt.Reward;
            Action<Action> performAction = (callback) =>
            {
                if (UISnakeLadderMainController.IsAuto)
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
        public void PerformGetBlockScore(EventSnakeLadderUIGetBlockScore evt)
        {
            Action<Action> performAction = (callback) =>
            {
                var position = BlockNodeList[evt.BlockIndex].transform.position;
                MainUI.FlyCarrot(position,evt.Score,callback);
                if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderAddScore))
                {
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SnakeLadderAddScore, null);
                }
            };
            MainUI.PushPerformAction(performAction);
        }
        public void PerformMoveSnake(EventSnakeLadderUIMoveSnake evt)
        {
            var startIndex = evt.StartIndex;
            var endIndex = evt.StartIndex + evt.MoveIndex;
            var startBlock = BlockNodeList[startIndex];
            var endBlock = BlockNodeList[endIndex];
            Action<Action> performAction = async (callback) =>
            {
                AudioManager.Instance.PlaySound(117);
                Player.SetBlockIndex(endIndex);
                SnakeAnimDic[evt.StartIndex].PlayAnimation("appear");
                Player.PlaySnakeHideAnim();
                await XUtility.WaitSeconds(0.17f);
                // Player.gameObject.SetActive(false);
                await XUtility.WaitSeconds(1f);
                // Player.gameObject.SetActive(true);
                Player.transform.position = endBlock.transform.position;
                Player.PlaySnakeShowAnim();
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }
        public void PerformMoveLadder(EventSnakeLadderUIMoveLadder evt)
        {
            var startIndex = evt.StartIndex;
            var endIndex = evt.StartIndex + evt.MoveIndex;
            var startBlock = BlockNodeList[startIndex];
            var endBlock = BlockNodeList[endIndex];
            Action<Action> performAction = async (callback) =>
            {
                AudioManager.Instance.PlaySound(116);
                Player.SetBlockIndex(endIndex);
                var animator = LadderAnimDic[evt.StartIndex];
                if (animator != null)
                {
                    animator.gameObject.SetActive(true);
                    animator.PlayAnimation("appear");   
                }
                await XUtility.WaitSeconds(0.2f);
                Player.PlayLadderAnim();
                Player.transform.DOMove(endBlock.transform.position, 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (animator != null)
                    {
                        animator.PlayAnimation("idle");
                        animator.gameObject.SetActive(false);
                    }
                    Player.PlayIdle();
                    Player.transform.position = endBlock.transform.position;
                    callback();
                });
            };
            MainUI.PushPerformAction(performAction);
        }
        public void PerformMoveStep(EventSnakeLadderUIMoveStep evt)
        {
            var startIndex = evt.StartIndex;
            var endIndex = evt.StartIndex + evt.MoveIndex;
            if (endIndex > BlockNodeList.Count - 1)
                endIndex = BlockNodeList.Count - 1;
            Action<Action> performAction = async (callback) =>
            {
                for (var i = startIndex; i < endIndex; i++)
                {
                    var targetIndex = i + 1;
                    var targetBlock = BlockNodeList[targetIndex];
                    var task = new TaskCompletionSource<bool>();
                    AudioManager.Instance.PlaySound(114);
                    Player.transform.DOJump(targetBlock.transform.position, SnakeLadderModel.Instance.GlobalConfig.JumpPower, 1, 0.3f).OnComplete(() =>
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

        public void PerformScoreMultiChange(EventSnakeLadderScoreMultiChange evt)
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
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderUIGetBlockReward>(PerformGetBlockReward);
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderUIGetBlockScore>(PerformGetBlockScore);
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderUIMoveSnake>(PerformMoveSnake);
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderUIMoveLadder>(PerformMoveLadder);
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderUIMoveStep>(PerformMoveStep);
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderScoreMultiChange>(PerformScoreMultiChange);
        }
    }
}