using Cysharp.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Screw.GameLogic;
using Screw.Module;
using Screw.UserData;

namespace Screw
{
    public class LevelStateHandler : ILogicEventHandler, ILevelFailHandler
    {
        private ScrewGameContext _context;

        public LevelStateHandler(ScrewGameContext context)
        {
            _context = context;
        }

        public int GetExecuteOrder()
        {
            return ExecuteOrder.LevelStateHandler;
        }

        public void OnLogicEvent(LogicEvent logicEvent, LogicEventParams eventParams)
        {
            switch (logicEvent)
            {
                case LogicEvent.ShowHardLevel:
                    UIModule.Instance.ShowUI(typeof(UIHardLevelPopup), _context);
                    break;
                case LogicEvent.EnterLevel:
                    if (ScrewGameLogic.Instance.IsNextLevelUnlockFeature() != GameFeatureType.None)
                        UIModule.Instance.ShowUI(typeof(UIBoosterFeatureUnlockPopup));
                    break;
                case LogicEvent.ActionFinish:
                case LogicEvent.DebugWin:
                    CheckLevelComplete();
                    break;
                case LogicEvent.ExitLevel:
                    // TODO
                    break;
                case LogicEvent.EnterBreakPanel:
                    _context.EnterBreakPanel();
                    break;
                case LogicEvent.ExitBreakPanel:
                    _context.ExitBreakPanel();
                    break;
                case LogicEvent.RefreshBooster:
                    _context.boostersView.RefreshUI();
                    break;
            }
        }

        private async void OnLevelWin()
        {
            if (_context is ScrewMiniGameContext)
            {
                ((ScrewMiniGameContext)_context)._action?.Invoke(_context.gameState == ScrewGameState.Win);
                return;
            }
            if (_context is ScrewGameKapiScrewContext)
            {
                ((ScrewGameKapiScrewContext)_context).OnWin();
                return;
            }
            
             EnergyData.Instance.AddEnergy(1, new GameBIManager.ItemChangeReasonArgs()
             {
                 reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ScrewPassLevel,
                 data1 = _context.levelIndex.ToString(),
             });
            
             CalculateLevelWinReward();
            
             if (_context is ScrewGameContext)
             {
                 ScrewGameLogic.Instance.UpdateUserLevelIndexToNext();
                 //错误注释
                 //StorageManager.Instance.GetStorage<StorageGlobal>().UnclaimedLevelReward.CollectOtherReward = true;
             }
             else
             {
                 //错误注释
                 //StorageManager.Instance.GetStorage<StorageGlobal>().UnclaimedLevelReward.CollectOtherReward = false;
             }
            
             //错误注释
             //GameApp.Get<ScrewGameSys>().UpdateWinStreak();
            
             var gameFeatureType = ScrewGameLogic.Instance.GetNextUnlockFeature(_context.levelIndex);
            
             UIModule.Instance.CloseWindow(typeof(UIConfirmLeaveLevelPopup));
            
             await OnBeforeWinLevelTransition();
             UIModule.Instance.ShowUI(typeof(UIScrewWinPopup), _context, gameFeatureType);
        }

        private void CalculateLevelWinReward()
        {
             var rewardCoin = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].PassLevel;
            
             //迁移报错注释
             //_context.biLevelInfo.LevelScore = (uint) rewardCoin;
            
             var rewardClover = DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].PassLevelClover;
            
             //迁移报错注释
             // StorageManager.Instance.GetStorage<StorageGlobal>().UnclaimedLevelReward.UnClaimedCoinCount = rewardCoin;
             // bool plotEnabled = GameApp.Get<PlotSys>().PlotSysIsEnabled();
             // if (plotEnabled && _context is not ScrewGameSpecialContext)
             //     StorageManager.Instance.GetStorage<StorageGlobal>().UnclaimedLevelReward.UnClaimedCloverCount = rewardClover;
             // else
             //     rewardClover = 0;
            
             //迁移报错注释
             // if (GameApp.Get<WeekChallengeSys>().IsUnlock() && _context is not ScrewGameSpecialContext)
             // {
             //     var jamCount = 0;
             //     foreach (var taskModel in _context.LevelModel.TaskModels)
             //         jamCount += taskModel.SlotCount;
             //
             //     StorageManager.Instance.GetStorage<StorageGlobal>().UnclaimedLevelReward.UnClaimedWeeklyCount =
             //         jamCount;
             //
             //     _context.biLevelInfo.CollectionCount = (uint) jamCount;
             // }
             //
             // StorageManager.Instance.GetStorage<StorageGlobal>().UnclaimedLevelReward.WatchedRv = false;
             //     
             // _context.biLevelInfo.LevelCoinScore = (uint)rewardCoin;
             //
             // _context.biLevelInfo.LevelCloverScore = (uint)rewardClover;
        }

        private void OnLevelFailed()
        {
             _context.Record.SetUseOther();
             UpdateFailedStatisticInfo(false);
            
             UIModule.Instance.CloseWindow(typeof(UIConfirmLeaveLevelPopup));
            
             switch (_context.failReason)
             {
                 case LevelFailReason.ConnectFailed:
                     ScrewUtility.WaitSeconds(0.5f, HandleFailPopup).Forget();
                     break;
                 case LevelFailReason.CollectableArea:
                     HandleFailPopup();
                     break;
                 case LevelFailReason.Timer:
                     UIModule.Instance.ShowUI(typeof(UITimesUpFail), _context);
                     break;
                 case LevelFailReason.BombFailed:
                 case LevelFailReason.IceFailed:
                 // 锁不可能输
                 // case LevelFailReason.LockFailed:
                 case LevelFailReason.ShutterFailed:
                 case LevelFailReason.TieFailed:
                     ScrewUtility.WaitSeconds(0.5f, () =>
                     {
                         if (_context is ScrewMiniGameContext)
                         {
                             ((ScrewMiniGameContext)_context)._action?.Invoke(false);
                         }
                         else if (_context is ScrewGameKapiScrewContext)
                         {
                             ((ScrewGameKapiScrewContext)_context).OnBlockerFail();
                         }
                         else
                         {
                             UIModule.Instance.ShowUI(typeof(UIBlockerFailedPopup), _context);
                         }
                     }).Forget();
                     break;
             }
        }

        private void HandleFailPopup()
        {
            if (_context is ScrewMiniGameContext)
            {
                ((ScrewMiniGameContext)_context)._action?.Invoke(false);
                return;
            }
            if (_context is ScrewGameKapiScrewContext)
            {
                ((ScrewGameKapiScrewContext)_context).OnFullFail();
                return;
            }
            
             if (_context.boostersHandler.GetHandler<ExtraSlotBoosterHandler>(BoosterType.ExtraSlot).CanUseInFailed())
                 UIModule.Instance.ShowUI(typeof(UIKeepPlayingPopup), this, _context);
             else if (_context is ScrewGameSpecialContext)
                 SceneFsm.mInstance.ChangeState(StatusType.ScrewHome);
             else
                 UIModule.Instance.ShowUI(typeof(UITryAgainPopup), _context);
        }

        private void UpdateFailedStatisticInfo(bool triggerFromExitLevel)
        {
            //迁移报错注释
            //var gameStatistic = StorageManager.Instance.GetStorage<StorageGlobal>().GameStatistic;
            
            // if (!gameStatistic.ContainsKey(_context.levelIndex))
            // {
            //     gameStatistic.Add(_context.levelIndex, new StorageFailedStatisticInfo());
            // }
            
            //避免失败信息被统计多次
            if (triggerFromExitLevel)
            {
                if (_context.failReason != LevelFailReason.Exit)
                {
                    return;
                }
            }
            
            // if (gameStatistic[_context.levelIndex].FailedInfo.ContainsKey(_context.failReason.ToString()))
            // {
            //     gameStatistic[_context.levelIndex].FailedInfo[_context.failReason.ToString()]++;
            // }
            // else
            // {
            //     gameStatistic[_context.levelIndex].FailedInfo.Add(_context.failReason.ToString(), 1);
            // }
        }

        public void CheckLevelComplete()
        {
            if (_context.actionController.HasActionInExecute())
                return;

            if (_context.gameState == ScrewGameState.Win)
            {
                OnLevelWin();
            }
            else if (_context.gameState == ScrewGameState.Fail)
            {
                OnLevelFailed();
            }
        }

        private async void OnGiveUp()
        {
            _context.SendLevelFailBi();

            if (_context.afterFailLevelHandlers != null && _context.afterFailLevelHandlers.Count > 0)
            {
                for (int i = 0; i < _context.afterFailLevelHandlers.Count; i++)
                {
                    await _context.afterFailLevelHandlers[i].Invoke(_context);
                }
            }

             if (_context is ScrewGameSpecialContext)
                 SceneFsm.mInstance.ChangeState(StatusType.ScrewHome);
             else
                 UIModule.Instance.ShowUI(typeof(UITryAgainPopup), _context, true);
        }

        public async void OnUserSelectPlayOn()
        {
            UIModule.Instance.EnableEventSystem = false;
            var extraSlotBoosterHandler =
                _context.boostersHandler.GetHandler<ExtraSlotBoosterHandler>(BoosterType.ExtraSlot);

            //迁移报错注释
            //_context.biLevelInfo.NoGridRespawnCount++;
            _context.SetFirstRespawnBi();

            if (_context.failReason == LevelFailReason.ConnectFailed)
            {
                // 如果是绳子导致的失败一并执行
                _context.LevelRevived(extraSlotBoosterHandler);
            }
            else
            {
                await extraSlotBoosterHandler.Use(false);
                await extraSlotBoosterHandler.Use(false);
                _context.hookContext.OnLogicEvent(LogicEvent.RefreshExtraSlot, null);
            }
            _context.gameState = ScrewGameState.InProgress;
            _context.failReason = LevelFailReason.None;

            _context.hookContext.OnLogicEvent(LogicEvent.BlockCheckFail, null);
            _context.hookContext.OnLogicEvent(LogicEvent.CheckTask, null);

            UIModule.Instance.EnableEventSystem = true;
        }

        public async void OnUserSelectGiveUp()
        {
            if (_context.levelFailedHandlers != null
                && _context.levelFailedHandlers.Count > 0)
            {
                for (var i = 0; i < _context.levelFailedHandlers.Count; i++)
                {
                    var playOn = await _context.levelFailedHandlers[i].Invoke();
                    if (playOn)
                    {
                        OnUserSelectPlayOn();
                        return;
                    }
                }
            }

            OnGiveUp();
        }
        
        private async UniTask OnBeforeWinLevelTransition()
        {
            if (_context.beforeWinLevelHandlers != null && _context.beforeWinLevelHandlers.Count > 0)
            {
                for (var i = 0; i < _context.beforeWinLevelHandlers.Count; i++)
                {
                    var toNext = await _context.beforeWinLevelHandlers[i].Invoke(_context);
                    if (!toNext)
                    {
                        return;
                    }
                }
            }
        }
    }
}