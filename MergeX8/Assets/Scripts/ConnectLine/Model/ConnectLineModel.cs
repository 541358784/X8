using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using OnePath;
using UnityEngine;

namespace ConnectLine.Model
{
    public class ConnectLineModel : Singleton<ConnectLineModel>
    {
        public int debugLevelId { get; set; }

        public TableConnectLineLevel _config;
        
        public StorageConnectLine ConnectLine
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().ConnectLine; }
        }
        
        public void InitModel(TableConnectLineLevel config)
        {
            _config = config;
        }
        
     

        public bool IsUnLock(TableConnectLineLevel config)
        {
            if (config == null)
                return false;
            
            if (config.unlockNodeNum <= 0)
                return true;

            if (IsPlayLevel(config))
                return true;

            return DecoManager.Instance.GetOwnedNodeNum() >= config.unlockNodeNum;
        }

        public bool IsPlayLevel(TableConnectLineLevel config)
        {
            if (config == null)
                return false;

            if (IsFinish(config))
                return true;

            return false;
        }
        
        public bool IsFinish(TableConnectLineLevel config)
        {
            if (config == null)
                return false;
            
            return IsFinish(config.id);
        }

        public bool IsFinish(int id)
        {
            return ConnectLine.FinishInfo.ContainsKey(id);
        }
        
        public void FinishLevel(int levelId)
        {
            if(ConnectLine.FinishInfo.ContainsKey(levelId))
                return;
            if (levelId == 1)
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventConnectLineFirstcomplete1);
                var bag = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find("Surface/Movie_Ship/ripple/SkeletonUtility-SkeletonRoot/root/chuan/bag");
                if (bag)
                    bag.gameObject.SetActive(false);
            }
            else if (levelId == 2)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventConnectLineFirstcomplete2);
            else if (levelId == 3)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventConnectLineFirstcomplete3);
            else if (levelId == 4)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventConnectLineFirstcomplete4);
            else if (levelId == 5)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventConnectLineFirstcomplete5);
            
            ConnectLine.FinishInfo.Add(levelId, true);
        }

        public bool HaveNoFinishLevel()
        {
            var configs = ASMR.Model.Instance.GetMiniGameItem(UIPopupGameTabulationController.MiniGameTypeTab.ConnectLine);
            if (configs == null)
                return false;
            
            foreach (var item in configs)
            {
                var TableOnePathLevel = ConnectLineConfigManager.Instance._configs.Find(a => a.id == item.configId);
                if(TableOnePathLevel == null)
                    continue;
                
                if (IsUnLock(TableOnePathLevel) && !IsFinish(TableOnePathLevel))
                    return true;
            }

            return false;
        }
        
        public bool IsShowHand()
        {
            for(int i = 0; i <  ConnectLineConfigManager.Instance._configs.Count; i++)
            {
                if (i == 3 && i == 4)
                {
                    if (IsUnLock(ConnectLineConfigManager.Instance._configs[i]) && !IsFinish(ConnectLineConfigManager.Instance._configs[i]))
                        return true;
                }
            }

            return false;
        }
    }
}