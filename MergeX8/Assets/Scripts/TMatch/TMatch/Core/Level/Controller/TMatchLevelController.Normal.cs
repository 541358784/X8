using System;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;

namespace TMatch
{


    public class TMatchNormalLevelController : SubSystem, IInitable, TMatchLevelController
    {
        private TMatchLevelData levelData;
        private TMatchLevelStateData levelStateData;

        public TMatchLevelData LevelData
        {
            get => levelData;
        }

        public TMatchLevelStateData LevelStateData
        {
            get => levelStateData;
        }
        public TMGameType GameType
        {
            get=>TMGameType.Normal;
        }

        public void Init()
        {
            levelData = new TMatchLevelData();
            levelData.Init();
            levelStateData = new TMatchLevelStateData();

            EventDispatcher.Instance.AddEventListener(EventEnum.POPUP_OPEN, OnPopupOpenAndCloseEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.POPUP_CLOSE, OnPopupOpenAndCloseEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_TARGET_FINISH, OnTargetFinishEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_TIMEOUT, OnTimeOutEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_SPACEOUT, OnSpaceOutEvt);
        }

        public void Release()
        {
            levelData.Release();

            EventDispatcher.Instance.RemoveEventListener(EventEnum.POPUP_OPEN, OnPopupOpenAndCloseEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.POPUP_CLOSE, OnPopupOpenAndCloseEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TARGET_FINISH, OnTargetFinishEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TIMEOUT, OnTimeOutEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_SPACEOUT, OnSpaceOutEvt);
        }
        private FsmParamTMatch param;

        public FsmParamTMatch Param()
        {
            return param;
        }
        public void Build(FsmParamTMatch enterParam)
        {
            param = enterParam;
            levelData.level = enterParam.level;
            levelData.layoutCfg = enterParam.layoutCfg;
            levelData.LastTimes = enterParam.layoutCfg.levelTimes;

            TMatchStateSystem.Instance.RegisterState(TMatchStateType.Prepare);
            TMatchStateSystem.Instance.RegisterState(TMatchStateType.Create);
            TMatchStateSystem.Instance.RegisterState(TMatchStateType.Play);
            TMatchStateSystem.Instance.RegisterState(TMatchStateType.Finish);
            TMatchStateSystem.Instance.RegisterState(TMatchStateType.Destory);

            EventDispatcher.Instance.DispatchEvent(
                new TMatchGameChangeStateEvent(TMatchStateType.Prepare,
                    new TMatchPrepareStateParam() {fsmParamTMatch = enterParam}));

            StorageManager.Instance.GetStorage<StorageTMatch>().MainLevelFailCnt++;

            if (!EnergyModel.Instance.IsEnergyUnlimited())
            {
                ItemModel.Instance.Cost((int) ResourceId.TMEnergy, 1, new DragonPlus.GameBIManager.ItemChangeReasonArgs
                {
                    data1 = enterParam.level.ToString(),
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.PlayLevelTm
                });
            }
            // EnergyModel.Instance.CostEnergy(1, new DragonPlus.GameBIManager.ItemChangeReasonArgs()
            // {
            //     reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.None,
            //     data1 = enterParam.level.ToString(),
            // });
        }

        private void OnPopupOpenAndCloseEvt(BaseEvent evt)
        {
            levelStateData.pause = UIViewSystem.Instance.HasPopup();
        }

        private void OnTargetFinishEvt(BaseEvent evt)
        {
            OnWin();
        }

        private void OnTimeOutEvt(BaseEvent evt)
        {
            if (levelData.isSettlement) return;

            UIViewSystem.Instance.Open<UITMatchTimeOutController>(new UitMatchTimeOutViewParam()
            {
                handleClick =
                    (b) =>
                    {
                        if (!b)
                        {
                            OnFail(1);
                        }
                    }
            });
        }

        private void OnSpaceOutEvt(BaseEvent evt)
        {
            if (levelData.isSettlement) return;

            UIViewSystem.Instance.Open<UITMatchSpaceOutController>(new UitMatchSpaceOutViewParam()
            {
                handleClick =
                    (b) =>
                    {
                        if (!b)
                        {
                            OnFail(2);
                        }
                    }
            });
        }

        public async void OnWin()
        {
            if (levelData.isSettlement) return;
            levelData.isSettlement = true;
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                .GameEventTmLevelEnd,data2:"1",data1: TMatchModel.Instance.GetMainLevel().ToString());
            UIViewSystem.Instance.Close<UITMatchTimeOutController>();
            UIViewSystem.Instance.Close<UITMatchSpaceOutController>();

            // if (RemoveAdModel.Instance.IsUnlock() && (!StorageManager.Instance.GetStorage<StorageTMatch>().RemoveAd.RemoveAd) && AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.TM_FINISH_GAME))
            // {
            //     AdSubSystem.Instance.PlayInterstital(ADConstDefine.TM_FINISH_GAME, b =>
            //     {
            //         RemoveAdModel.Instance.TryToAutoOpen();
            //     });
            // }
            
            levelData.win = true;
            // DragonPlus.GameBIManager.SendGameWinEvent(TMatchSystem.LevelController.LevelData.level);
            SendWinLevelInfo();
            SendTrackingEvent(TMatchSystem.LevelController.LevelData.level);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.TMATCH_GAME_WIN_BEFORE_ADD_MAIN_LEVEL);
            TMatchModel.Instance.MainLevelFinish();

            if (!ItemModel.Instance.IsNumMax((int) ResourceId.TMEnergy))
            {
                ItemModel.Instance.Add((int) ResourceId.TMEnergy, 1, new DragonPlus.GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug
                });
            }

            // EnergyModel.Instance.AddEnergy(1, 
            //     new DragonPlus.GameBIManager.ItemChangeReasonArgs()
            //     {
            //         reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.None,
            //         data1 = TMatchSystem.LevelController.LevelData.level.ToString(),
            //     });

            EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_WIN);
            await Task.Delay(850);
            UIViewSystem.Instance.Open<UITMatchWinController>();
        }

        public void SendWinLevelInfo()
        {
            try
            {
                GameBIManager.Instance.LevelInfo.LeftTimeCount =
                    (uint) TMatchSystem.LevelController.LevelData.LastTimes;
                GameBIManager.Instance.LevelInfo.LevelTime = GameBIManager.Instance.LevelInfo.TotalTime -
                                                             (uint) TMatchSystem.LevelController.LevelData.LastTimes;
                GameBIManager.Instance.LevelInfo.LevelResult = "pass";
                GameBIManager.Instance.LevelInfo.LevelScore = (uint) StarCurrencyModel.Instance.GetStarCnt(
                    TMatchSystem.LevelController.LevelData.layoutCfg.levelTimes,
                    TMatchSystem.LevelController.LevelData.LastTimes);
                GameBIManager.Instance.LevelInfo.CollectionCount +=
                    (uint) CollectCoinGateView.CollectCnt(TMatchSystem.LevelController.LevelData.tripleItems);
                GameBIManager.Instance.LevelInfo.CollectionCount +=
                    (uint) CollectDiamondGateView.CollectCnt(TMatchSystem.LevelController.LevelData.tripleItems);
                // GameBIManager.Instance.LevelInfo.ChallengeCount +=
                //     (uint)WeeklyChallengeGateView.CollectCnt(TMatchSystem.LevelController.LevelData.tripleItems);
                // GameBIManager.Instance.LevelInfo.TeamChestCount +=
                //     (uint)CollectGuildGateView.CollectCnt(TMatchSystem.LevelController.LevelData.tripleItems);
                GameBIManager.Instance.SendLevelInfoEvent();
            }
            catch (Exception e)
            {

            }
        }

        public void OnFail(uint failReason)
        {
            if (levelData.isSettlement) return;
            levelData.isSettlement = true;
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                .GameEventTmLevelEnd,data2:"0",data1: TMatchModel.Instance.GetMainLevel().ToString());
            // if (RemoveAdModel.Instance.IsUnlock() && (!StorageManager.Instance.GetStorage<StorageTMatch>().RemoveAd.RemoveAd) && AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.TM_FINISH_FAIL_GAME))
            // {
            //     AdSubSystem.Instance.PlayInterstital(ADConstDefine.TM_FINISH_FAIL_GAME, b =>
            //     {
            //         RemoveAdModel.Instance.TryToAutoOpen();
            //     });
            // }
            UIViewSystem.Instance.Open<UITMatchFailController>();
            EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_FAIL);

            // send level info bi
            GameBIManager.Instance.LevelInfo.LeftTimeCount = (uint) TMatchSystem.LevelController.LevelData.LastTimes;
            GameBIManager.Instance.LevelInfo.LevelTime = GameBIManager.Instance.LevelInfo.TotalTime -
                                                         (uint) TMatchSystem.LevelController.LevelData.LastTimes;
            GameBIManager.Instance.LevelInfo.LevelTime = (uint) TMatchSystem.LevelController.LevelData.LastTimes;
            GameBIManager.Instance.LevelInfo.LevelResult = "fail";
            GameBIManager.Instance.LevelInfo.FailReason = failReason;
            GameBIManager.Instance.SendLevelInfoEvent();
        }

        public void SendTrackingEvent(int level)
        {
            switch (level)
            {
                case 30:
                    DragonPlus.GameBIManager.onThirdPartyTracking("GAME_EVENT_PASS_LV30");
                    break;
                case 40:
                    DragonPlus.GameBIManager.onThirdPartyTracking("GAME_EVENT_PASS_LV40");
                    break;
                case 50:
                    DragonPlus.GameBIManager.onThirdPartyTracking("GAME_EVENT_PASS_LV50");
                    break;
                case 70:
                    DragonPlus.GameBIManager.onThirdPartyTracking("GAME_EVENT_PASS_LV70");
                    break;
                case 100:
                    DragonPlus.GameBIManager.onThirdPartyTracking("GAME_EVENT_PASS_LV100");
                    break;
                case 150:
                    DragonPlus.GameBIManager.onThirdPartyTracking("GAME_EVENT_PASS_LV150");
                    break;
                case 200:
                    DragonPlus.GameBIManager.onThirdPartyTracking("GAME_EVENT_PASS_LV200");
                    break;
                case 300:
                    DragonPlus.GameBIManager.onThirdPartyTracking("GAME_EVENT_PASS_LV300");
                    break;
            }
        }
    }
}