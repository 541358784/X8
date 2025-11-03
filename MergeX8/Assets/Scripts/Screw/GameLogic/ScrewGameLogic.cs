using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Screw;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Screw.Module;
using Screw.UserData;
using UnityEngine;

namespace Screw.GameLogic
{
    public class ScrewGameLogic : Manager<ScrewGameLogic>, IRunOnce
    {
        public StorageScrew _screwStorage
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageScrew>();
            }
        }
    
        public ScrewGameContext context;
        
        private List<TableFeatureUnlockInfo> _featureUnlockInfos;
    
        private List<TableLevels> _levelsList;
        private List<TableLoopLevels> _loopLevelsList;
        private int _memoryWinStreak = 0;
    
        public Vector3 WorldCameraPosition =>new Vector3(0, -1, -25);
        public float WorldCameraOrthographicSize => 14f;

        public int ScrewLayer = 7;

        public bool IsWin
        {
            get
            {
                return context.gameState == ScrewGameState.Win;
            }
        }
        public void OnRunOnce()
        {
            _levelsList = DragonPlus.Config.Screw.GameConfigManager.Instance.TableLevelsList;
            _loopLevelsList = DragonPlus.Config.Screw.GameConfigManager.Instance.TableLoopLevelsList;

            InitMainLevel();
            
            _featureUnlockInfos = DragonPlus.Config.Screw.GameConfigManager.Instance.TableFeatureUnlockInfoList;
            _featureUnlockInfos.Sort((a, b) =>
            {
                return a.UnlockLevel - b.UnlockLevel;
            });
            
            UserData.UserData.Instance.OnRunOnce();
            EnergyData.Instance.OnRunOnce();
        }

        private void InitMainLevel()
        {
            if(_screwStorage.MainLevelIndex <= 0)
                _screwStorage.MainLevelIndex = 1;
        }
        
        public int GetMainLevelIndex()
        {
            InitMainLevel();
            
            return _screwStorage.MainLevelIndex;
        }

        public bool SendBi = true;
        public void EnterScrewGame(int targetLevelId, Action<object> action = null,bool sendBi = true)
        {
            SendBi = sendBi;
            if (sendBi)
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventScrewLevelEnter,GetMainLevelIndex().ToString());
            if (targetLevelId == -1)
            {
                context = new ScrewGameContext();
                context.OnEnterLevel(GetMainLevelIndex(), GetLevelId(GetMainLevelIndex()));
            }
            else if (UIKapiScrewMainController.Instance)
            {
                context = new ScrewGameKapiScrewContext();
                context.OnEnterLevel(GetMainLevelIndex(), targetLevelId);
                ((ScrewGameKapiScrewContext)context)._action = action;
            }
            else
            {
                bool isMiniGame = DragonPlus.Config.Screw.GameConfigManager.Instance.TableMiniGameLevelsList[0].LevelIds.Contains(targetLevelId);
                if (isMiniGame)
                {
                    context = new ScrewMiniGameContext();
                    context.OnEnterLevel(GetMainLevelIndex(), targetLevelId);
                    ((ScrewMiniGameContext)context)._action = action;
                }
                else
                {
                    context = new ScrewGameSpecialContext();
                    context.OnEnterLevel(GetMainLevelIndex(), targetLevelId);
                }
            }
    
            if(targetLevelId == -1)
                CostEnergy(GetMainLevelIndex());
            // _memoryWinStreak = GameApp.Get<UserProfileSys>().GetCurrentWinStreak();
            // EventBus.Dispatch(new EventEnterScrewGame(context));
            // StorageManager.Instance.GetStorage<StorageGlobal>().PlayerTrack.AddEnterScrewCount();
        }
        
        public void ExitGame(bool quiet = false)
        {
            if (context.gameState != ScrewGameState.Win)
            {
                _memoryWinStreak = 0;
            }
    
            PoolModule.Instance.Release();
            // GameApp.Get<UserProfileSys>().SetCurrentWinStreak(_memoryWinStreak);
            // context.biLevelInfo.CurrentWin = (uint) _memoryWinStreak;
            // EventBus.Dispatch(new EventExitScrewGame(context));
    
            if (context != null)
                context.OnExitLevel();
            context = null;
        }
    
        public bool LevelIsHardDifficulty(int levelIndex)
        {
            if (levelIndex > _levelsList.Count)
            {
                var loopIndex = (levelIndex - _levelsList.Count - 1) % _loopLevelsList.Count;
                return _loopLevelsList[loopIndex].Difficulty;
            }
            
            return _levelsList[levelIndex - 1].Difficulty;
        }
    
        public bool IsTimeLimitLevel(int levelIndex)
        {
            if (levelIndex > _levelsList.Count)
            {
                var loopIndex = (levelIndex - _levelsList.Count - 1) % _loopLevelsList.Count;
                return _loopLevelsList[loopIndex].IsTimeLevel;
            }
            return _levelsList[levelIndex - 1].IsTimeLevel;
        }
    
        public int GetTimeLimit(int levelIndex)
        {
            if (levelIndex > _levelsList.Count)
            {
                var loopIndex = (levelIndex - _levelsList.Count - 1) % _loopLevelsList.Count;
                return _loopLevelsList[loopIndex].TimeLimit;
            }
            return _levelsList[levelIndex - 1].TimeLimit;
        }
    
        public void UpdateUserLevelIndexToNext()
        {
            _screwStorage.MainLevelIndex++;
        }
    
        public void UpdateWinStreak()
        {
            _memoryWinStreak++;
            //PlayerPrefs.SetInt(TaskType.CheckAndClaimLevelReward.ToString(), 0);
        }
    
        private void CostEnergy(int levelIndex)
        {
            EnergyData.Instance.CostEnergy(1, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ScrewPlayLevel,
                data1 = levelIndex.ToString(),
            });
        }
    
        private int GetLevelId(int levelIndex)
        {
            if (levelIndex > _levelsList.Count)
            {
                var loopIndex = (levelIndex - _levelsList.Count - 1) % _loopLevelsList.Count;
                return _loopLevelsList[loopIndex].LevelId;
            }
            
            return _levelsList[levelIndex - 1].LevelId;
        }
        
        public int GetFeatureUnlockLevel(GameFeatureType featureType)
        {
            foreach (var featureUnlockInfo in _featureUnlockInfos)
            {
                if (featureUnlockInfo.FeatureType == (int) featureType)
                {
                    return featureUnlockInfo.UnlockLevel;
                }
            }
    
            return -1;
        }
        
        public GameFeatureType GetNextUnlockFeature(int levelIndex = -1)
        {
            var currentLevel = levelIndex <= 0 ? _screwStorage.MainLevelIndex : levelIndex;
            for (var i = 0; i < _featureUnlockInfos.Count; i++)
            {
                if (_featureUnlockInfos[i].UnlockLevel > currentLevel)
                {
                    return (GameFeatureType) _featureUnlockInfos[i].FeatureType;
                }
            }
    
            return GameFeatureType.None;
        }
    
        public GameFeatureType IsNextLevelUnlockFeature()
        {
            var currentLevel = _screwStorage.MainLevelIndex;
            for (var i = 0; i < _featureUnlockInfos.Count; i++)
            {
                if (_featureUnlockInfos[i].UnlockLevel == currentLevel)
                {
                    return (GameFeatureType) _featureUnlockInfos[i].FeatureType;;
                }
            }
    
            return GameFeatureType.None;
        }
    
        public string GetUnlockFeatureProgressDesc(int currentLevel, bool checkEqualLevel = false)
        {
            for (var i = 0; i < _featureUnlockInfos.Count; i++)
            {
                var unlockLevel = _featureUnlockInfos[i].UnlockLevel;
    
                if (unlockLevel > currentLevel
                    || (checkEqualLevel && unlockLevel == currentLevel))
                {
                    if (i == 0)
                    {
                        return
                            $"{currentLevel - 1}/{unlockLevel - 1}";
                    }
    
                    var lastUnlockLevel =_featureUnlockInfos[i - 1].UnlockLevel;
                    return
                        $"{currentLevel - lastUnlockLevel}/{unlockLevel - lastUnlockLevel}";
                }
            }
    
            return "";
        }
        
        public float GetUnlockFeatureProgress(int currentLevel, bool checkEqualLevel = false)
        {
            for (var i = 0; i < _featureUnlockInfos.Count; i++)
            {
                var unlockLevel = _featureUnlockInfos[i].UnlockLevel;
                if (unlockLevel > currentLevel
                    || (checkEqualLevel && unlockLevel == currentLevel))
                {
                    if (i == 0)
                    {
                        return (float) (currentLevel-1) / (unlockLevel - 1);
                    }
    
                    var lastUnlockLevel = _featureUnlockInfos[i - 1].UnlockLevel;
                    return
                        (float) (currentLevel - lastUnlockLevel) /
                        (unlockLevel - lastUnlockLevel);
                }
            }
            
            return 0;
        }
        
        public void DebugPassWin()
        {
            if (context != null)
            {
                if (context.gameState == ScrewGameState.InProgress)
                {
                    context.gameState = ScrewGameState.Win;
                    context.hookContext.OnLogicEvent(LogicEvent.DebugWin, null);
                }
            }
        }
    
        public uint GetPlayCount()
        {
            return context.GetPlayCount();
        }
    
        public bool GetGameIsFailed()
        {
            if (context == null)
                return false;
            return context.GetGameIsFailed();
        }
    }
}