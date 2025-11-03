using System.Collections;
using System.Collections.Generic;
using Framework.Utils;
using UnityEngine;

namespace MiniGame
{
    public struct EventEnterMinigameLevel : IEvent
    {
        public int subLevelId;
        public int levelId;

        public EventEnterMinigameLevel(int subLevelId, int levelId)
        {
            this.subLevelId = subLevelId;
            this.levelId = levelId;
        }
    }

    public struct EventMiniStoryDirectFinish : IEvent
    {
        public int subLevelId;
        public int levelId;

        public EventMiniStoryDirectFinish(int subLevelId, int levelId)
        {
            this.subLevelId = subLevelId;
            this.levelId = levelId;
        }
    }

    public struct EventEnterASMRLevel : IEvent
    {
        public int subLevelId;
        public int levelId;

        public EventEnterASMRLevel(int subLevelId, int levelId)
        {
            this.subLevelId = subLevelId;
            this.levelId = levelId;
        }
    }

    /// <summary>
    /// 章节剧情故事播放完毕
    /// </summary>
    public struct EventMiniGameStoryFinished : IEvent
    {
    }

    /// <summary>
    /// 玩家点击 MiniGame 三选一选项事件
    /// </summary>
    public struct EventMiniGameSelectionClicked : IEvent
    {
        public int selectionIndex; // 玩家所点击选项的索引，从 0 开始
        public int subLevelId;
        public int levelId;

        public EventMiniGameSelectionClicked(int selectionIndex, int subLevelId, int levelId)
        {
            this.selectionIndex = selectionIndex;
            this.subLevelId = subLevelId;
            this.levelId = levelId;
        }
    }

    /// <summary>
    /// MiniGame 弹出胜利弹窗的时候
    /// </summary>
    public struct EventMiniGameLevelWin : IEvent
    {
    }

    /// <summary>
    /// 点击 MiniGame 胜利界面领取奖励的时候
    /// </summary>
    public struct EventMiniGameLevelWinClaimed : IEvent
    {
    }

    /// <summary>
    /// MiniGame 弹出重试弹窗的时候
    /// </summary>
    public struct EventMiniGameLevelFailed : IEvent
    {
    }

    /// <summary>
    /// 点击 MiniGame 重试界面进行返回重试的时候
    /// </summary>
    public struct EventMiniGameLevelFailRetryClicked : IEvent
    {
    }

    /// <summary>
    /// 章节2特殊处理
    /// </summary>
    public struct EventMiniGameSpecialHandleChapter2 : IEvent
    {
    }
    
    /// <summary>
    /// 章节2特殊处理
    /// </summary>
    public struct EventMiniGameSpecialHandleFinishChapter2 : IEvent
    {
    }
    
    public struct EventGuideMaskClick : IEvent
    {}
    
    public struct EventFinishCurrHomeTask : IEvent
    {}
    
    
    public class EventMiniGame
    {
        public const string MINIGAME_SETSHOWSTATUS = "MINIGAME_SETSHOWSTATUS";
        public const string MINIGAME_BGM = "MINIGAME_BGM";
        
        public const string MINIGAME_IMPLEMENTFINISH = "MINIGAME_IMPLEMENTFINISH";
    }
}