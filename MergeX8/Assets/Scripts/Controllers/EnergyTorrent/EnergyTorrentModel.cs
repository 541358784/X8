using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay.UI.Store.Vip.Model;

namespace Gameplay.UI.EnergyTorrent
{
    public class EnergyTorrentModel : Manager<EnergyTorrentModel>
    {
        private static EnergyTorrentModel _instance;
        public static EnergyTorrentModel Instance => _instance ?? (_instance = new EnergyTorrentModel());
        
        private StorageEnergyTorrent _storageEnergyTorrent;

        public StorageEnergyTorrent StorageEnergyTorrent
        {
            get
            {
                if (_storageEnergyTorrent == null)
                {
                    _storageEnergyTorrent= StorageManager.Instance.GetStorage<StorageHome>().EnergyTorrent;
                }
           
                return _storageEnergyTorrent;
            }
        }
        public EnergyTorrentModel()
        {
    
            EventDispatcher.Instance.AddEvent<EventUserDataAddRes>((e) =>
            {
                OnAddRes(e.ResId,e.Count);
            });
        }
        public void OnAddRes(UserData.ResourceId resId,int count)
        {       
            if (resId == UserData.ResourceId.Energy)
            {
                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                {
                    CoroutineManager.Instance.StartCoroutine(AutoPopupManager.AutoPopupManager.Instance.GamePayingPopUIViewLogic());
                }
            }
        }
        public bool IsOpen()
        {
            return GetMultiply() > 1;
        }
        public bool IsOpenx8()
        {
            return GetMultiply()==8;
        }

        public bool IsUnlock()
        {
            if (VipStoreModel.Instance.VipLevel() >= 5)
                return true;
            
            int level = ExperenceModel.Instance.GetLevel();
            if (level >= GlobalConfigManager.Instance.GetNumValue("energy_frenzy_level") ||
                (int)StorageManager.Instance.GetStorage<StorageCommon>().RevenueUSDCents >  GlobalConfigManager.Instance.GetNumValue("energy_frenzy_quota")*100.0f
                  &&level>= GlobalConfigManager.Instance.GetNumValue("energy_frenzy_charge_level"))
                return true;
            return false;
        }

        public bool IsUnlock4Multiply()
        {
            if (VipStoreModel.Instance.VipLevel() >= 5)
                return true;

            int level = ExperenceModel.Instance.GetLevel();
            if (level >= GlobalConfigManager.Instance.GetNumValue("energy_4frenzy_level") ||
                (int)StorageManager.Instance.GetStorage<StorageCommon>().RevenueUSDCents >  GlobalConfigManager.Instance.GetNumValue("energy_4frenzy_quota")*100.0f
                  &&level>= GlobalConfigManager.Instance.GetNumValue("energy_4frenzy_charge_level"))
                return true;
            return false;
        }     
        public bool IsUnlock8Multiply()
        {
            if (VipStoreModel.Instance.VipLevel() >= 5)
                return true;

            int level = ExperenceModel.Instance.GetLevel();
            if (level >= GlobalConfigManager.Instance.GetNumValue("energy_8frenzy_level") ||
                (int)StorageManager.Instance.GetStorage<StorageCommon>().RevenueUSDCents >  GlobalConfigManager.Instance.GetNumValue("energy_8frenzy_quota")*100.0f
                  &&level>= GlobalConfigManager.Instance.GetNumValue("energy_8frenzy_charge_level"))
                return true;
            return false;
        }

        public void SetMultiply()
        {
            if (StorageEnergyTorrent.Multiply == 1 || StorageEnergyTorrent.Multiply == 0)
            {
                StorageEnergyTorrent.Multiply = 2;
            }else if (StorageEnergyTorrent.Multiply == 2)
            {
                StorageEnergyTorrent.Multiply = 4;
            }
            else if (StorageEnergyTorrent.Multiply == 4)
            {
                StorageEnergyTorrent.Multiply = 1;
            }
            else
            {
                StorageEnergyTorrent.Multiply = 1;
            }
            EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND);

        }

        public int GetMultiply()
        {
            if (StorageEnergyTorrent.Multiply == 0)
            {
                if(StorageEnergyTorrent.IsOpen)
                    StorageEnergyTorrent.Multiply = 2;
                else
                    StorageEnergyTorrent.Multiply = 1;
            }
            return StorageEnergyTorrent.Multiply ;
        }

        public void SetOpenStateX8()
        {
            if (IsOpenx8())
            {
                StorageEnergyTorrent.IsOpened = false;
                StorageEnergyTorrent.Multiply = 1;
            }
            else
            {
                StorageEnergyTorrent.IsOpened = true;
                StorageEnergyTorrent.Multiply = 8;
            }
    
            EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND);
        }
        public void SetCloseStateX8()
        {
            StorageEnergyTorrent.IsOpened = false;
            StorageEnergyTorrent.Multiply = 1;
            EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND);
        }
        public void ReSeStateX8()
        {
            StorageEnergyTorrent.IsOpened = false;
            StorageEnergyTorrent.Multiply = 4;
            EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND);
        }

        public void SetOpenState()
        {
            if (!(GetMultiply() > 1))
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyFrenzyFristTiemOpen);
                StorageEnergyTorrent.IsOpened = true;
                StorageEnergyTorrent.Multiply = 2;
            }
            else
            {
                StorageEnergyTorrent.Multiply = 1;
            }
            StorageEnergyTorrent.IsOpen = !StorageEnergyTorrent.IsOpen;
            if (StorageEnergyTorrent.IsOpen)
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyOn);
            }
            else
            {
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEnergyOff);
            }
            EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND);
        }
        public void RecordShowStart()
        {
            _storageEnergyTorrent.IsShowStart=true;
        }

        public bool IsShowStart()
        {
            return _storageEnergyTorrent.IsShowStart;
        }

        public long GetLeftTime()
        { 
            var serverTime = APIManager.Instance.GetServerTime();
            var config = GlobalConfigManager.Instance.GetTableEnergyTorrent();
            if ((long)serverTime - StorageEnergyTorrent.MaxStartTime < (long)config.limitTime * 60 * 1000)
            {
                return (long)config.limitTime * 60 * 1000-((long)serverTime - StorageEnergyTorrent.MaxStartTime);
            }
            return 0;
        }
        public bool CheckX8()
        {
            if (!IsUnlock8Multiply())
                return false;

            if (VipStoreModel.Instance.VipLevel() >= 5)
            {
                if (!EnergyTorrentModel.Instance.StorageEnergyTorrent.IsShowStart)
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentMainX8);
                    return true;
                }

                return false;
            }
            
            var config = GlobalConfigManager.Instance.GetTableEnergyTorrent();
            var serverTime = APIManager.Instance.GetServerTime();
            if (StorageEnergyTorrent.MaxStartTime == 0 || (long)serverTime - StorageEnergyTorrent.MaxStartTime >=
                (long)config.limitTime * 60 * 1000)
            {
                if (UserData.Instance.GetRes(UserData.ResourceId.Energy) >= config.energyCount)
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentMainX8);
                    StorageEnergyTorrent.MaxStartTime = (long)serverTime;
                    return true;
                }
            }

            return false;
        }

        public bool CanShowUIFix()
        {
            return true;
        }

        public bool CanShowUI()
        {
            if (!IsUnlock())
                return false;
            if (CheckX8())
                return true;
            if (EnergyTorrentModel.Instance.StorageEnergyTorrent.IsShowStart)
                return false;
            
            UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentStart);

            return true;
        }
    }
}