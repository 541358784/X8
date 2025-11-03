using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;
using Screw;
using Screw.Configs;
using Screw.GameLogic;
using Screw.Module;
using UnityEngine;
using UnityEngine.U2D;

namespace Screw
{
    public class ScrewGameContext:ILevelFailHandler
    {
        public ScrewGameState gameState;
        public LevelFailReason failReason;
        public int failCount = -1;//不包含安全区满的失败

        public bool IsFirstClicked = true;

        private Dictionary<LevelFailReason, List<ScrewModel>> blockerFailModels = new Dictionary<LevelFailReason, List<ScrewModel>>();

        public int levelIndex;
        public int levelId;

        public BlockersHandler blockersHandler;
        public MembersHandler membersHandler;
        public BoostersHandler boostersHandler;

        public ScrewActionController actionController;

        private LevelModel levelModel;

        public LevelModel LevelModel => levelModel;

        protected ScrewLevelView levelView;
        
        public ScrewHeaderView headerView;

        public BoostersView boostersView;

        public LogicEventHookContext hookContext;

        public GameTimerHandler gameTimer;

        public Vector3 ShadowOffset = new Vector3(-0.09f, -0.09f, 0);

        public Dictionary<ColorType, ColorSetting.ColorData> ColorDic;

        public List<Func<ScrewGameContext, UniTask<bool>>> beforeWinLevelHandlers;
        public List<Func<ScrewGameContext, UniTask<bool>>> afterWinLevelHandlers;
        public List<Func<UniTask<bool>>> exitLevelHandlers;
        public List<Func<UniTask<bool>>> levelFailedHandlers;

        public List<Func<ScrewGameContext, UniTask<bool>>> afterFailLevelHandlers;
        public List<Func<ScrewGameContext, UniTask<bool>>> afterExitLevelHandlers;

        //迁移报错注释
        // public List<Func<UIWidget, bool>> exitLevelPopupHandlers;
        // public List<Func<UIWidget, bool>> levelFailedPopupHandlers;
        //
        // public BiEventScrewscapes.Types.LevelInfo biLevelInfo;

        protected SpriteAtlas _screwAtlas;
        protected Dictionary<string, Sprite> _panelSprites;

        private float _levelTime = 0f;

        public RecordAction Record;

        public ScrewGameContext()
        {
            hookContext = new LogicEventHookContext();
            hookContext.AddEventHandler(new LevelStateHandler(this));
            hookContext.AddEventHandler(new LevelTaskHandler(this));
            hookContext.AddEventHandler(new LevelShieldHandler(this));
            
            blockersHandler = new BlockersHandler(this);
            membersHandler = new MembersHandler(this);

            boostersHandler = new BoostersHandler(this);

            actionController = new ScrewActionController(this);
            
            hookContext.AddEventHandler(membersHandler);
            hookContext.AddEventHandler(boostersHandler);
        }
        
        public virtual void OnEnterLevel(int inLevelIndex, int inLevelId)
        {
            _screwAtlas = AssetModule.Instance.LoadSpriteAtlas(ConstName.ScrewAtlas);
            levelIndex = inLevelIndex;
            levelId = inLevelId;

            LoadLevelSetting();
            LoadLevelModel();
            LoadLevelView();

            gameState = ScrewGameState.InProgress;

            Record = new RecordAction(this);
            levelView.OnEnterLevel();
             // var levelDifficulty = ScrewGameLogic.Instance.LevelIsHardDifficulty(levelIndex);
            
             // if (levelDifficulty)
             //     hookContext.OnLogicEvent(LogicEvent.ShowHardLevel, null);
             // else
             //     hookContext.OnLogicEvent(LogicEvent.EnterLevel, null);
             hookContext.OnLogicEvent(LogicEvent.EnterLevel, null);
            
             PoolModule.Instance.CreatePool(ConstName.HammerEffect, 2);

            // InitBiLevelInfo(levelDifficulty);
            UpdateScheduler.Instance.HookUpdate(Tick);
        }

        public Sprite GetPanelSprite(string name)
        {
            if (_panelSprites == null)
            {
                _panelSprites = new Dictionary<string, Sprite>();
            }
            if (!_panelSprites.ContainsKey(name))
            {
                _panelSprites.Add(name, AssetModule.Instance.LoadAsset<Sprite>($"Screw/NamedTextures/Panel/{name}"));
            }

            return _panelSprites[name];
        }

        protected void Tick()
        {
            _levelTime += Time.deltaTime;
        }

        public Sprite GetSprite(string name)
        {
            return _screwAtlas.GetSprite(name);
        }

        protected void LoadLevelSetting()
        {
            ColorDic = ColorSetting.Instance.ColorMap;
        }

        public void OnExitLevel()
        {
            _screwAtlas = null;
            _panelSprites.Clear();
            UpdateScheduler.Instance.UnhookUpdate(Tick);

            hookContext.OnLogicEvent(LogicEvent.ExitLevel, null);

            boostersView.OnExitLevel();
            headerView.ClearListener();

            levelView.ExitLevel();
        }

        protected void LoadLevelModel(bool isSpecial = false)
        {
            //levelId = 990000;
            var textAsset = AssetModule.Instance.LoadLevelData(levelId);
            if (textAsset == null)
            {
                Debug.LogError("Load Level Config Error Is Nil Id: " + levelId);
                return;
            }
            var levelLayout = JsonConvert.DeserializeObject<LevelLayout>(textAsset.text);

            var currentLevelId = levelLayout.levelId;
            Dictionary<int, LayerModel> layerModels = new Dictionary<int, LayerModel>();
            for (int i = 0; i < levelLayout.layers.Count; i++)
            {
                var layerT = levelLayout.layers[i];
                Dictionary<int, PanelBodyModel> panelBodyModels = new Dictionary<int, PanelBodyModel>();
                foreach (var panelBodyT in layerT.panels)
                {
                    Dictionary<int, HoleModel> holeModels = new Dictionary<int, HoleModel>();
                    foreach (var holeT in panelBodyT.holes)
                    {
                        var holeId = holeT.instanceId;
                        var holePos = holeT.position;
                        HoleModel holeModel = new HoleModel(holeId, holePos);
                        holeModels.Add(holeT.instanceId, holeModel);
                    }

                    PanelBodyModel panelBodyModel = new PanelBodyModel(i, panelBodyT, holeModels);
                    panelBodyModels.Add(panelBodyT.instanceId, panelBodyModel);
                }

                Dictionary<int, ScrewModel> screwModels = new Dictionary<int, ScrewModel>();
                foreach (var screwT in layerT.screws)
                {
                    ScrewModel screwModel = new ScrewModel(i, screwT.instanceID, screwT.position,
                        screwT.colorType, screwT.radius, screwT.screwBlocks, screwT.shape, this);
                    screwModels.Add(screwT.instanceID, screwModel);
                }

                Dictionary<int, ShieldModel> shieldModels = new Dictionary<int, ShieldModel>();
                foreach (var shield in layerT.shields)
                {
                    ShieldModel shieldModel = new ShieldModel(i, shield);
                    shieldModels.Add(shield.instanceId, shieldModel);
                }
                
                LayerModel layerModel = new LayerModel(panelBodyModels, screwModels, shieldModels, i);
                layerModels.Add(i, layerModel);
            }
            
             if (ScrewGameLogic.Instance.IsTimeLimitLevel(levelIndex) && !isSpecial)
             {
                 gameTimer = new GameTimerHandler(this, ScrewGameLogic.Instance.GetTimeLimit(levelIndex));
                 hookContext.AddEventHandler(gameTimer);
             }

            List<OrderModel> tasks = new List<OrderModel>();
            foreach (var slotModuleDataT in levelLayout.orders)
            {
                tasks.Add(new OrderModel(slotModuleDataT.colorType, slotModuleDataT.slotCount, slotModuleDataT.shapes));
            }
            levelModel = new LevelModel(currentLevelId, "", levelLayout.guidePosition, layerModels, tasks);
        }

        protected void LoadLevelView()
        {
            levelView = ScrewLevelView.LoadLevel(this, levelModel);
            
            Vector3 position =ScrewGameLogic.Instance.WorldCameraPosition;
            position.y = 0;
            position.z = 0;
            levelView.GetRoot().position = position;
        }

        public int GetLayer(int layerId)
        {
            switch (layerId)
            {
                case 0:
                    return LayerMask.NameToLayer("Layer0");
                case 1:
                    return LayerMask.NameToLayer("Layer1");
                case 2:
                    return LayerMask.NameToLayer("Layer2");
                case 3:
                    return LayerMask.NameToLayer("Layer3");
                case 4:
                    return LayerMask.NameToLayer("Layer4");
                case 5:
                    return LayerMask.NameToLayer("Layer5");
                case 6:
                    return LayerMask.NameToLayer("Layer6");
                case 7:
                    return LayerMask.NameToLayer("Layer7");
                case 8:
                    return LayerMask.NameToLayer("Layer8");
                case 9:
                    return LayerMask.NameToLayer("Layer9");
            }
            
            return LayerMask.NameToLayer("Layer0");
        }

        public Transform StorageScrew(ScrewModel screwModel)
        {
            return levelView.StorageScrew(screwModel);
        }

        public void SetSlotState(Transform target)
        {
            levelView.SetSlotState(target);
        }
        public void SetModelSlotState(Transform target)
        {
            levelView.SetModelSlotState(target);
        }

        public ScrewView GetScrewView(ScrewModel screwModel)
        {
            return levelView.GetScrewView(screwModel);
        }

        public ScrewView GetScrewView(int screwId)
        {
            return levelView.GetScrewView(screwId);
        }

        public ScrewModel GetScrewModel(int screwId)
        {
            foreach (var layerModel in levelModel.LayerModels)
            {
                ScrewModel model; 
                layerModel.Value.ScrewModels.TryGetValue(screwId, out model);
                if (model!=null)
                {
                    return model;
                }
            }
            return null;
        }
        
        public T GetBlockerView<T>(ScrewBlocker type, ScrewModel screwModel) where T : BaseBlockerView
        {
            return levelView.GetBlockerView<T>(type, screwModel);
        }
        
        public List<BaseBlockerView> GetAllBlockerView(ScrewBlocker type)
        {
            return levelView.GetAllBlockerView(type);
        }

        public void AddSlot(bool playEff)
        {
            //迁移报错注释
            //biLevelInfo.Bonusslot++;
            levelView.AddSlot(playEff);
        }

        public void UseBoosterTwoTask()
        {
            //迁移报错注释
            //biLevelInfo.Bonusmodule++;
            levelView.UseBoosterTwoTask();
        }
        public bool GetCouldUseTwoTaskBooster()
        {
            return levelView.GetCouldUseTwoTaskBooster();
        }

        public async UniTask CheckTask()
        {
            await levelView.CheckMoveToNextTask();
            levelView.CheckCollectAreaToCurrentTask();
        }

        public void RefreshTaskStatus(ScrewModel screwModel)
        {
            levelView.RefreshTaskStatus(screwModel);
        }

        public bool SlotAreaFull()
        {
            return levelView.SlotAreaFull();
        }
        
        public void HookBeforeWinLevelHandler(Func<ScrewGameContext, UniTask<bool>> onWinLevelHandler)
        {
            if (beforeWinLevelHandlers == null)
            {
                beforeWinLevelHandlers = new List<Func<ScrewGameContext, UniTask<bool>>>();
            }

            if (!beforeWinLevelHandlers.Contains(onWinLevelHandler))
            {
                beforeWinLevelHandlers.Add(onWinLevelHandler);
            }
        }
        public void HookAfterWinLevelHandler(Func<ScrewGameContext, UniTask<bool>> onWinLevelHandler)
        {
            if (afterWinLevelHandlers == null)
            {
                afterWinLevelHandlers = new List<Func<ScrewGameContext, UniTask<bool>>>();
            }

            if (!afterWinLevelHandlers.Contains(onWinLevelHandler))
            {
                afterWinLevelHandlers.Add(onWinLevelHandler);
            }
        }

        public void HookAfterFailLevelHandlers(Func<ScrewGameContext, UniTask<bool>> onWinLevelHandler)
        {
            if (afterFailLevelHandlers == null)
            {
                afterFailLevelHandlers = new List<Func<ScrewGameContext, UniTask<bool>>>();
            }

            if (!afterFailLevelHandlers.Contains(onWinLevelHandler))
            {
                afterFailLevelHandlers.Add(onWinLevelHandler);
            }
        }

        public void HookAfterExitLevelHandlers(Func<ScrewGameContext, UniTask<bool>> onWinLevelHandler)
        {
            if (afterExitLevelHandlers == null)
            {
                afterExitLevelHandlers = new List<Func<ScrewGameContext, UniTask<bool>>>();
            }

            if (!afterExitLevelHandlers.Contains(onWinLevelHandler))
            {
                afterExitLevelHandlers.Add(onWinLevelHandler);
            }
        }

        public void HookExitLevelHandler(Func<UniTask<bool>> onExitLevelHandler)
        {
            if (exitLevelHandlers == null)
            {
                exitLevelHandlers = new List<Func<UniTask<bool>>>();
            }

            if (!exitLevelHandlers.Contains(onExitLevelHandler))
            {
                exitLevelHandlers.Add(onExitLevelHandler);
            }
        }

        public void HookLevelFailExitHandler(Func<UniTask<bool>> onLevelFailHandler)
        {
            if (levelFailedHandlers == null)
            {
                levelFailedHandlers = new List<Func<UniTask<bool>>>();
            }

            if (!levelFailedHandlers.Contains(onLevelFailHandler))
            {
                levelFailedHandlers.Add(onLevelFailHandler);
            }
        }

        //迁移报错注释
        // public void HookExitLevelPopupHandler(Func<UIWidget, bool> onExitLevelHandler)
        // {
        //     if (exitLevelPopupHandlers == null)
        //     {
        //         exitLevelPopupHandlers = new List<Func<UIWidget, bool>>();
        //     }
        //
        //     if (!exitLevelPopupHandlers.Contains(onExitLevelHandler))
        //     {
        //         exitLevelPopupHandlers.Add(onExitLevelHandler);
        //     }
        // }
        //
        // public void HookLevelFailExitPopupHandler(Func<UIWidget, bool> onLevelFailHandler)
        // {
        //     if (levelFailedPopupHandlers == null)
        //     {
        //         levelFailedPopupHandlers = new List<Func<UIWidget, bool>>();
        //     }
        //
        //     if (!levelFailedPopupHandlers.Contains(onLevelFailHandler))
        //     {
        //         levelFailedPopupHandlers.Add(onLevelFailHandler);
        //     }
        // }

        public void AddFailBlocker(LevelFailReason failType, ScrewModel screwModel)
        {
            if (!blockerFailModels.ContainsKey(failType))
            {
                blockerFailModels.Add(failType, new List<ScrewModel>());
            }

            if (!blockerFailModels[failType].Contains(screwModel))
            {
                blockerFailModels[failType].Add(screwModel);
            }
        }

        public bool CheckConnectLineScrewJam()
        {
            LevelFailReason failType = LevelFailReason.ConnectFailed;

            bool hasConnectLine = false;
            if (blockerFailModels.ContainsKey(failType))
            {
                foreach (var screwModel in blockerFailModels[failType])
                {
                    if (screwModel.HasBlocker)
                    {
                        if (screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.ConnectBlocker, out BaseBlockerModel model))
                        {
                            if (model is ConnectBlockerModel connectBlockerModel)
                            {
                                var connectBlockerView = GetBlockerView<ConnectBlockerView>(ScrewBlocker.ConnectBlocker, screwModel);
                                connectBlockerView.DisConnect();
                                actionController.AddMoveAction(screwModel, GetScrewView(screwModel)).Forget();
                                hasConnectLine = true;
                            }
                        }
                    }
                }
                blockerFailModels[failType].Clear();
            }

            return hasConnectLine;
        }

        public async void LevelRevived(ExtraSlotBoosterHandler extraSlotBoosterHandler)
        {
            if (blockerFailModels.Count > 0)
            {
                LevelFailReason failType = failReason;
                if (blockerFailModels.ContainsKey(failType))
                {
                    foreach (var screwModel in blockerFailModels[failType])
                    {
                        if (screwModel.HasBlocker)
                        {
                            if (screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.ConnectBlocker, out BaseBlockerModel model))
                            {
                                if (model is ConnectBlockerModel connectBlockerModel)
                                {
                                    await extraSlotBoosterHandler.Use(false);
                                    await extraSlotBoosterHandler.Use(false);
                                    hookContext.OnLogicEvent(LogicEvent.RefreshExtraSlot, null);

                                    var connectBlockerView = GetBlockerView<ConnectBlockerView>(ScrewBlocker.ConnectBlocker, screwModel);
                                    connectBlockerView.DisConnect();
                                    actionController.AddMoveAction(screwModel, GetScrewView(screwModel)).Forget();
                                }
                            }
                            else
                            {
                                blockersHandler.CompleteBlockerInRevive(screwModel);
                            }
                        }
                    }
                    blockerFailModels[failType].Clear();
                }
            }
            
            failReason = LevelFailReason.None;
            gameState = ScrewGameState.InProgress;
        }

        public void SetLevelFailReasonCollectableArea()
        {
            if (failReason != LevelFailReason.ConnectFailed)
            {
                failReason = LevelFailReason.CollectableArea;
            }
            gameState = ScrewGameState.Fail;
        }

        public void EnterBreakPanel()
        {
            levelView.EnterBreakPanel();
        }

        public void ExitBreakPanel()
        {
            levelView.ExitBreakPanel();
        }

        public void PlayHammerAni(Vector3 screenPosition)
        {
             var flyObj = PoolModule.Instance.SpawnGameObject(ConstName.HammerEffect);
             flyObj.transform.SetParent(UIModule.Instance.UiRoot, false);
            
             // 屏幕坐标转UI坐标
             Vector2 localPosition;
             RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)UIModule.Instance.UiRoot, screenPosition, UIModule.Instance.WorldCamera, out localPosition);
            
             flyObj.transform.GetComponent<RectTransform>().anchoredPosition = localPosition;
             ScrewUtility.WaitSeconds(2, () => { PoolModule.Instance.RecycleGameObject(flyObj); }).Forget();
        }

        public int GetCollectJamCount()
        {
            return levelView.GetCollectJamCount();
        }

        #region Bi

        private bool _isFirstRespawn = true;
        protected void InitBiLevelInfo(bool levelDifficulty)
        {
            //迁移报错注释
            // biLevelInfo = new BiEventScrewscapes.Types.LevelInfo();
            // biLevelInfo.LevelId = (uint)levelId;
            // biLevelInfo.LevelCount = (uint) levelIndex;
            // biLevelInfo.LevelDifficulty = (uint) levelDifficulty;
            // biLevelInfo.LevelResult = "enter";
            //
            // var jamCount = 0;
            // var taskCount = 0;
            // foreach (var taskModel in LevelModel.TaskModels)
            // {
            //     jamCount += taskModel.SlotCount;
            //     taskCount++;
            // }
            //
            // biLevelInfo.InitialNailCount = (uint) jamCount;
            // biLevelInfo.ExcessNailCount = (uint) jamCount;
            //
            // if (gameTimer != null)
            // {
            //     biLevelInfo.LevelLimitTime = (uint)gameTimer.GetLeftTime();
            //     biLevelInfo.ExcessTimeCount = (uint)gameTimer.GetLeftTime();
            //     biLevelInfo.LevelTime = biLevelInfo.LevelLimitTime - biLevelInfo.ExcessTimeCount;
            // }
            // else
            // {
            //     biLevelInfo.LevelTime = (uint) _levelTime;
            // }
            //
            // biLevelInfo.Bonusslot = 0;
            // biLevelInfo.Hammer = 0;
            // biLevelInfo.Bonusmodule = 0;
            //
            // biLevelInfo.CurrentWin = (uint) GameApp.Get<UserProfileSys>().GetCurrentWinStreak();
            //
            // biLevelInfo.CupCout = GetGoldenLeagueCup();
            // biLevelInfo.OverToolboxGridNoilCount = string.Empty;
            // biLevelInfo.TotalToolboxCount = (uint) taskCount;
            // biLevelInfo.FinishToolboxCount = 0;
            //
            // biLevelInfo.GridNoilCount = 0;
            // biLevelInfo.GridCount = 5;
            //
            // UpdateStaticInfo(levelIndex);
            //
            // BIHelper.SendEvent(biLevelInfo);
        }

        private void UpdateStaticInfo(int gameLevelIndex)
        {
            //迁移报错注释
            // var gameStatistic = StorageManager.Instance.GetStorage<StorageGlobal>().GameStatistic;
            //
            // if (!gameStatistic.ContainsKey(gameLevelIndex))
            // {
            //     gameStatistic.Add(gameLevelIndex, new StorageFailedStatisticInfo());
            // }
            //
            // if (gameStatistic[gameLevelIndex].FailedInfo.ContainsKey("PlayCount"))
            // {
            //     gameStatistic[gameLevelIndex].FailedInfo["PlayCount"]++;
            // }
            // else
            // {
            //     gameStatistic[gameLevelIndex].FailedInfo.Add("PlayCount", 1);
            // }
            //
            // biLevelInfo.EnterTime = (uint)gameStatistic[gameLevelIndex].FailedInfo["PlayCount"];
            // biLevelInfo.EnergyInfinite = GameApp.Get<EnergySys>().IsInfiniteEnergy();
        }

        private Dictionary<int, uint> taskCompleteSafeAreaJamCountDic;

        public void SetCompleteTaskSafeAreaCount(int index, uint count)
        {
            if (taskCompleteSafeAreaJamCountDic == null)
            {
                taskCompleteSafeAreaJamCountDic = new Dictionary<int, uint>();
            }

            if (taskCompleteSafeAreaJamCountDic.ContainsKey(index))
            {
                return;
            }

            taskCompleteSafeAreaJamCountDic.Add(index, count);

            //迁移报错注释
            //biLevelInfo.FinishToolboxCount++;
        }

        private int GetGoldenLeagueCup()
        {
            return 0;
            //迁移报错注释
            // var activity = GameApp.Get<ActivitySys>().GetActivity<Activity_GoldenLeague>(ActivityType.GoldenLeague);
            // if (activity == null)
            //     return 0;
            //
            // if (!activity.IsActivityOpened() && !activity.IsActivityInReward())
            //     return 0;
            //
            // return activity.GetCurrentWinStreak();
        }

        public void SendLevelWinBi(float adRatio)
        {
            //迁移报错注释
            // var currentLevel = levelIndex;
            // if (currentLevel <= 20)
            // {
            //     var eventType = (BiEventScrewscapes.Types.GameEventType) Enum.Parse(
            //         typeof(BiEventScrewscapes.Types.GameEventType), $"GameEventFunnelLevel{currentLevel}Success");
            //     BIHelper.SendGameEvent(eventType);
            // }
            //
            // if (taskCompleteSafeAreaJamCountDic != null)
            //     biLevelInfo.OverToolboxGridNoilCount = DictionaryToString(taskCompleteSafeAreaJamCountDic);
            // biLevelInfo.LevelResult = "pass";
            // biLevelInfo.Rv = adRatio;
            // if (gameTimer != null)
            // {
            //     biLevelInfo.ExcessTimeCount = (uint) gameTimer.GetLeftTime();
            //     biLevelInfo.LevelTime = biLevelInfo.LevelLimitTime - biLevelInfo.ExcessTimeCount;
            // }
            // else
            // {
            //     biLevelInfo.LevelTime = (uint) _levelTime;
            // }
            //
            // biLevelInfo.GridNoilCount = 0;
            // biLevelInfo.GridCount = 5;
            //
            // BIHelper.SendEvent(biLevelInfo);
        }

        public void SendLevelFailBi()
        {
            if (ScrewGameLogic.Instance.SendBi)
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventScrewLevelEnd,levelIndex.ToString(),"fail");
            //迁移报错注释
            // biLevelInfo.LevelResult = "fail";
            // if (taskCompleteSafeAreaJamCountDic != null)
            //     biLevelInfo.OverToolboxGridNoilCount = DictionaryToString(taskCompleteSafeAreaJamCountDic);
            // biLevelInfo.CollectionCount = (uint) GetCollectJamCount();
            // if (gameTimer != null)
            // {
            //     biLevelInfo.ExcessTimeCount = (uint) gameTimer.GetLeftTime();
            //     biLevelInfo.LevelTime = biLevelInfo.LevelLimitTime - biLevelInfo.ExcessTimeCount;
            // }
            // else
            // {
            //     biLevelInfo.LevelTime = (uint) _levelTime;
            // }
            //
            // biLevelInfo.ExcessNailCount = biLevelInfo.InitialNailCount - biLevelInfo.CollectionCount;
            //
            // biLevelInfo.GridNoilCount = levelView.GetSafeAreaStorageJamCount();
            // biLevelInfo.GridCount = levelView.GetAllAreaCount();
            //
            // switch (failReason)
            // {
            //     case LevelFailReason.Timer:
            //         biLevelInfo.FailReason = 2;
            //         break;
            //     case LevelFailReason.ConnectFailed:
            //     case LevelFailReason.CollectableArea:
            //         biLevelInfo.FailReason = 1;
            //         break;
            //     case LevelFailReason.BombFailed:
            //         biLevelInfo.FailReason = 3;
            //         break;
            //     case LevelFailReason.IceFailed:
            //         biLevelInfo.FailReason = 5;
            //         break;
            //     case LevelFailReason.ShutterFailed:
            //         biLevelInfo.FailReason = 4;
            //         break;
            // }
            // BIHelper.SendEvent(biLevelInfo);
        }

        public void SendLevelQuitBi()
        {
            if (ScrewGameLogic.Instance.SendBi)
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventScrewLevelEnd,levelIndex.ToString(),"quit");
            //迁移报错注释
            // biLevelInfo.LevelResult = "quit";
            // if (taskCompleteSafeAreaJamCountDic != null)
            //     biLevelInfo.OverToolboxGridNoilCount = DictionaryToString(taskCompleteSafeAreaJamCountDic);
            // biLevelInfo.CollectionCount = (uint) GetCollectJamCount();
            // if (gameTimer != null)
            // {
            //     biLevelInfo.ExcessTimeCount = (uint) gameTimer.GetLeftTime();
            //     biLevelInfo.LevelTime = biLevelInfo.LevelLimitTime - biLevelInfo.ExcessTimeCount;
            // }
            // else
            // {
            //     biLevelInfo.LevelTime = (uint) _levelTime;
            // }
            // biLevelInfo.ExcessNailCount = biLevelInfo.InitialNailCount - biLevelInfo.CollectionCount;
            //
            // biLevelInfo.GridNoilCount = levelView.GetSafeAreaStorageJamCount();
            // biLevelInfo.GridCount = levelView.GetAllAreaCount();
            // BIHelper.SendEvent(biLevelInfo);
        }

        public void SetFirstRespawnBi()
        {
            if (_isFirstRespawn)
            {
                _isFirstRespawn = false;
                //迁移报错注释
                // biLevelInfo.FirstRespawnNoilThing = (uint) GetCollectJamCount() - levelView.GetSafeAreaStorageJamCount();
                // biLevelInfo.FirstRespawnToolboxThing = biLevelInfo.FinishToolboxCount;
            }
        }

        private string DictionaryToString(Dictionary<int, uint> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, uint> kvp in dictionary)
            {
                sb.AppendFormat("{0}: {1}", kvp.Key, kvp.Value);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        #endregion

        public uint GetPlayCount()
        {
            return 0;
            //迁移报错注释
            //return biLevelInfo.EnterTime;
        }

        public bool IsMovingTask()
        {
            return levelView.IsMovingTask();
        }

        public bool HasPanelMoving()
        {
            return levelView.HasPanelMoving();
        }

        public bool GetGameIsFailed()
        {
            return gameState == ScrewGameState.Fail;
        }

        public void  RefreshShield()
        {
           levelView.RefreshShield();
        }
        
        public async void OnUserSelectGiveUp()
        {
            if (levelFailedHandlers != null
                && levelFailedHandlers.Count > 0)
            {
                for (var i = 0; i < levelFailedHandlers.Count; i++)
                {
                    var playOn = await levelFailedHandlers[i].Invoke();
                    if (playOn)
                    {
                        OnUserSelectPlayOn();
                        return;
                    }
                }
            }

            OnGiveUp();
        }
        public async void OnUserSelectPlayOn()
        {
            UIModule.Instance.EnableEventSystem = false;
            var extraSlotBoosterHandler =
                boostersHandler.GetHandler<ExtraSlotBoosterHandler>(BoosterType.ExtraSlot);

            //迁移报错注释
            //_context.biLevelInfo.NoGridRespawnCount++;
            SetFirstRespawnBi();

            if (failReason == LevelFailReason.ConnectFailed)
            {
                // 如果是绳子导致的失败一并执行
                LevelRevived(extraSlotBoosterHandler);
            }
            else
            {
                await extraSlotBoosterHandler.Use(false);
                await extraSlotBoosterHandler.Use(false);
                hookContext.OnLogicEvent(LogicEvent.RefreshExtraSlot, null);
            }
            gameState = ScrewGameState.InProgress;
            failReason = LevelFailReason.None;

            hookContext.OnLogicEvent(LogicEvent.BlockCheckFail, null);
            hookContext.OnLogicEvent(LogicEvent.CheckTask, null);

            UIModule.Instance.EnableEventSystem = true;
        }
        
        public virtual async void OnGiveUp()
        {
            SendLevelFailBi();

            if (afterFailLevelHandlers != null && afterFailLevelHandlers.Count > 0)
            {
                for (int i = 0; i < afterFailLevelHandlers.Count; i++)
                {
                    await afterFailLevelHandlers[i].Invoke(this);
                }
            }

            if (this is ScrewGameSpecialContext)
                SceneFsm.mInstance.ChangeState(StatusType.ScrewHome);
            else
                UIModule.Instance.ShowUI(typeof(UITryAgainPopup), this, true);
        }
    }
}