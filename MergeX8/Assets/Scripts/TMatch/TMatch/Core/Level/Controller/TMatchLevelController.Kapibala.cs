using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;

namespace TMatch
{


    public class TMatchKapibalaLevelController : SubSystem, IInitable, TMatchLevelController
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
            get=>TMGameType.Kapibala;
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
            PropCostState.Clear();
            // if (!EnergyModel.Instance.IsEnergyUnlimited())
            // {
            //     ItemModel.Instance.Cost((int) ResourceId.TMEnergy, 1, new DragonPlus.GameBIManager.ItemChangeReasonArgs
            //     {
            //         data1 = enterParam.level.ToString(),
            //         reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.PlayLevelTm
            //     });
            // }
            
            // EnergyModel.Instance.CostEnergy(1, new DragonPlus.GameBIManager.ItemChangeReasonArgs()
            // {
            //     reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.None,
            //     data1 = enterParam.level.ToString(),
            // });
        }

        private void OnPopupOpenAndCloseEvt(BaseEvent evt)
        {
            levelStateData.pause = UIViewSystem.Instance.HasPopup() || UIPopupKapibalaGiftBagController.Instance;
        }

        private void OnTargetFinishEvt(BaseEvent evt)
        {
            OnWin();
        }

        private void OnTimeOutEvt(BaseEvent evt)
        {
            if (levelData.isSettlement) return;

            UIViewSystem.Instance.Open<UIPopupKapibalaReviveController>(new UIPopupKapibalaReviveParam()
            {
                failType = 1,
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

            UIViewSystem.Instance.Open<UIPopupKapibalaReviveController>(new UIPopupKapibalaReviveParam()
            {
                failType = 2,
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

        public Dictionary<int, int> PropCostState = new Dictionary<int, int>();

        public void CostProp(int prop, int count)
        {
            PropCostState.TryAdd(prop, 0);
            PropCostState[prop] += count;
        }

        public Dictionary<string, string> PropCostStateToString()
        {
            var str = new Dictionary<string, string>();
            foreach (var pair in PropCostState)
            {
                str.Add(pair.Key.ToString(),pair.Value.ToString());
            }
            return str;
        }

        public async void OnWin()
        {
            if (levelData.isSettlement) return;
            levelData.isSettlement = true;
            // DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
            //     .GameEventTmLevelEnd,data2:"1",data1: TMatchModel.Instance.GetMainLevel().ToString());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapibalaLevelEnd,
                data1:KapibalaModel.Instance.Storage.BigLevel.ToString(),
                data2:KapibalaModel.Instance.Storage.PlayingSmallLevel.ToString(),
                data3:"Win",
                extras:PropCostStateToString());
            UIViewSystem.Instance.Close<UIPopupKapibalaReviveController>();
            levelData.win = true;
            // SendWinLevelInfo();
            // SendTrackingEvent(TMatchSystem.LevelController.LevelData.level);
            // EventDispatcher.Instance.DispatchEventImmediately(EventEnum.TMATCH_GAME_WIN_BEFORE_ADD_MAIN_LEVEL);
            // TMatchModel.Instance.MainLevelFinish();
            

            // EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_WIN);
            
            KapibalaModel.Instance.DealWin();
            
            await Task.Delay(850);
            UIViewSystem.Instance.Open<UIPopupKapibalaWinController>();
        }
        public void OnFail(uint failReason)
        {
            if (levelData.isSettlement) return;
            levelData.isSettlement = true;
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKapibalaLevelEnd,
                data1:KapibalaModel.Instance.Storage.BigLevel.ToString(),
                data2:KapibalaModel.Instance.Storage.PlayingSmallLevel.ToString(),
                data3:"Fail",
                extras:PropCostStateToString());
            KapibalaModel.Instance.DealFail();
            UIViewSystem.Instance.Open<UIPopupKapibalaReviveFailController>();
            // EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_FAIL);
        }
    }
}