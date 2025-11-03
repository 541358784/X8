using System;
using System.Linq;
using System.Collections.Generic;
using ABTest;
using ASMR;
using Decoration;
using DragonPlus;
using DragonPlus.Config.MiniGame;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework.Utils;
using Gameplay;
using Manager;
using Scripts.UI;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace MiniGame
{
    public enum ChapterType
    {
        New = 1,
        Normal = 2,
    }

    public partial class MiniGameModel
    {
        private SpineSkinSwitcherTrack _track;
        private SpineSkinSwitcherAsset _asset;
        private SpineSkinSwitcherBehaviour _switcher;
        
        public static MiniGameModel Instance { get; } = new();

        public List<MiniGameChapter> ChapterConfigList;
        private List<MiniGameLevel> _levelConfigList;
        private List<MiniGameSelection> _selectionConfigList;

        public static readonly string MiniGameAtlas = "MiniGameAtlas";
            
        private MiniGameModel()
        {
        }

        public void Init()
        {
            Framework.UI.UIManager.Instance.Init();
            
            ChapterConfigList = MiniGameConfigManager.Instance.MiniGameChapterList;
            _selectionConfigList = MiniGameConfigManager.Instance.MiniGameSelectionList;

            _levelConfigList = MiniGameConfigManager.Instance.MiniGameLevelList;

            ASMRModel.Instance.Init();
            
            ChapterConfigList.ForEach(a=>GetChapterStorage(a.Id));
        }

        public bool IsOpen()
        {
            if (PlayerPrefs.HasKey("OpenMiniGame"))
            {
                if (PlayerPrefs.GetString("OpenMiniGame") == "true")
                    return true;
                return false;
            }

            // if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
            //     return true;
            
            return ABTestManager.Instance.IsMiniGameOpened();
            // var country = StorageManager.Instance.GetStorage<StorageCommon>().Country;
            // var isChina = country.ToLower().Equals("cn");
            //
            // return !isChina;
        }

        public int GetResIdByChapterId(int chapterId, ChapterType type)
        {
            var config = GetChapterConfig(chapterId);
            return config.ResId;
        }

        public MiniGameChapter GetChapterConfig(int chapterId)
        {
            return ChapterConfigList.Find(x => x.Id == chapterId);
        }

        public MiniGameLevel GetLevelConfig(int levelId)
        {
            var configList = _levelConfigList;

            return configList.Find(x => x.Id == levelId);
        }

        public MiniGameSelection GetSubLevelConfig(int subLevelId)
        {
            return _selectionConfigList.Find(x => x.Id == subLevelId);
        }

        public int GetMinUnFinishedChapterId()
        {
            var minId = int.MaxValue;

            foreach (var kv in StorageManager.Instance.GetStorage<StorageMiniGameVersion>().Chapters)
            {
                if (!kv.Value.Claimed) minId = Mathf.Min(minId, kv.Value.Id);
            }

            return minId;
        }

        public List<int> GetChapterLevels(int chapterId)
        {
            var configList = _levelConfigList;

            return configList
                .Where(level => level.MiniGameId == chapterId)
                .Select(level => level.Id)
                .ToList();
        }

        public int GetTheLastLevelInChapter(int chapterId)
        {
            return GetChapterLevels(chapterId).Count > 0 ? GetChapterLevels(chapterId).Max() : 0;
        }

        public bool HaveNewLevelUnlocked()
        {
            var next = StorageManager.Instance.GetStorage<StorageMiniGameVersion>().LastEnterUnlockLevelId + 1;
            if (next > GetMaxLevelId())
            {
                return false;
            }

            if (StorageManager.Instance.GetStorage<StorageMiniGameVersion>().LastEnterUnlockLevelId == GetTheLastLevelInChapter(GetMinUnFinishedChapterId()))
            {
                return false;
            }

            var cfg = GetLevelConfig(next);
            if (cfg == null)
                return false;
            
            return UserData.Instance.CanAford(cfg.CostId, cfg.CostCount);
        }

        public List<int> GetLevelAnswerBySubLevelId(int subLevelId)
        {
            return GetSubLevelConfig(subLevelId).SelectResult;
        }

        public int GetMaxLevelId()
        {
            var configList = _levelConfigList;

            return configList.Max(x => x.Id);
        }

        public bool IsMiniGameAllFinish()
        {
            foreach (var config in ChapterConfigList)
            {
                var storage = GetChapterStorage(config.Id);
                if (!storage.Claimed) return false;
            }

            return true;
        }

        public PlayableAsset LoadLevelPlayableAsset(int chapter, int step, int clickIndex, ChapterType type)
        {
            var bundleId = GetResIdByChapterId(chapter, type);
            return ResourcesManager.Instance.LoadResource<PlayableAsset>($"NewMiniGame/MiniGame/Chapters/Chapter{bundleId}/Prefab/Chapter_{step}_{clickIndex}");
        }

        public PlayableAsset LoadStoryPlayableAsset(int chapter, int storyIndex, ChapterType type)
        {
            var bundleId = GetResIdByChapterId(chapter, type);
            return ResourcesManager.Instance.LoadResource<PlayableAsset>($"NewMiniGame/MiniGame/Chapters/Chapter{bundleId}/Prefab/Chapter_Story_{storyIndex}");
        }

        public PlayableAsset LoadChapterFinishIdleAsset(int chapterId, ChapterType type)
        {
            var bundleId = GetResIdByChapterId(chapterId, type);
            return ResourcesManager.Instance.LoadResource<PlayableAsset>($"NewMiniGame/MiniGame/Chapters/Chapter{bundleId}/Prefab/Chapter_FinishIdle");
        }
        
        public PlayableAsset LoadChapterSpecialAsset(int chapterId, ChapterType type)
        {
            var bundleId = GetResIdByChapterId(chapterId, type);
            return ResourcesManager.Instance.LoadResource<PlayableAsset>($"NewMiniGame/MiniGame/Chapters/Chapter{bundleId}/Prefab/Chapter_9_0");
        }

        public PlayableAsset LoadChapterSpecialAsset8(int chapterId, ChapterType type)
        {
            var bundleId = GetResIdByChapterId(chapterId, type);
            return ResourcesManager.Instance.LoadResource<PlayableAsset>($"NewMiniGame/MiniGame/Chapters/Chapter{bundleId}/Prefab/Chapter_8_0");
        }
        public string GetBGMPath(int chapterId)
        {
            return "NewMiniGame/ASMR/Common/Audio/" + ChapterConfigList.Find(x => x.Id == chapterId).BgmName;
        }

        public StorageChapter GetChapterStorage(int chapterId)
        {
            if (!StorageManager.Instance.GetStorage<StorageMiniGameVersion>().Chapters.TryGetValue(chapterId, out var storage))
            {
                storage = new StorageChapter();
                storage.Id = chapterId;
                StorageManager.Instance.GetStorage<StorageMiniGameVersion>().Chapters.Add(chapterId, storage);
            }

            return storage;
        }

        public StorageLevel GetLevelStorage(int chaperId, int levelId)
        {
            var storage = GetChapterStorage(chaperId);
            if (!storage.LevelsDic.TryGetValue(levelId, out var level))
            {
                level = new StorageLevel();
                level.Id = levelId;
                storage.LevelsDic.Add(levelId, level);
            }

            return level;
        }

        public ResData ClaimChapterReward(int chapterId)
        {
            var cfg = GetChapterConfig(chapterId);
            var rewardId = cfg.ChapterRewardId;
            var rewardCnt = cfg.ChapterRewardCnt;

            var rewardItems = new ResData(rewardId, rewardCnt);
            UserData.Instance.AddRes(rewardItems, new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MinigameGet});

            if (!UserData.Instance.IsResource(cfg.ChapterRewardId))
            {
                TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(cfg.ChapterRewardId);
                if (mergeItemConfig != null)
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMinigameGet,
                        itemAId = mergeItemConfig.id,
                        ItemALevel = mergeItemConfig.level,
                        isChange = true,
                    });
                }
            }
            
            var storage = GetChapterStorage(chapterId);
            storage.Claimed = true;

            return rewardItems;
        }

        public ResData ClaimLevelReward(int chapterId, int levelId)
        {
            var levelCfg = GetLevelConfig(levelId);
            var data = new ResData(levelCfg.RewardId, levelCfg.RewardCount);

            var level = GetLevelStorage(chapterId, levelId);
            level.Claimed = true;

            UserData.Instance.AddRes(data, new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MinigameGet});

            EventBus.Send<EventMinigameProgressChanged>();

            return data;
        }

        public int GetMinUnfinishLevelInChapter(int chapterId)
        {
            var levels = GetChapterLevels(chapterId);

            var minLevel = int.MaxValue;
            foreach (var levelId in levels)
            {
                var level = GetLevelStorage(chapterId, levelId);
                if (!level.Claimed) minLevel = Mathf.Min(minLevel, level.Id);
            }

            return minLevel;
        }

        public int GetMaxFinishLevelInChapter(int chapterId)
        {
            var levels = GetChapterLevels(chapterId);

            var maxLevel = int.MinValue;
            foreach (var levelId in levels)
            {
                var level = GetLevelStorage(chapterId, levelId);
                if (level.Claimed) maxLevel = Mathf.Max(maxLevel, level.Id);
            }

            return maxLevel;
        }

        public int CurrentChapter
        {
            get
            {
                if (StorageManager.Instance.GetStorage<StorageMiniGameVersion>().CurrentChapter == 0)
                    StorageManager.Instance.GetStorage<StorageMiniGameVersion>().CurrentChapter = 1;
                
                return StorageManager.Instance.GetStorage<StorageMiniGameVersion>().CurrentChapter;
            }
            set
            {
                StorageManager.Instance.GetStorage<StorageMiniGameVersion>().CurrentChapter = value;
            }
        }
    }

    public struct EventMinigameProgressChanged : Framework.Utils.IEvent
    {
    }
}