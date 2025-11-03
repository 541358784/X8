using Decoration;
using Decoration.DaysManager;
using DragonPlus;
using DragonPlus.Config.DigTrench;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace DigTrench
{
    public class DigTrenchEntryControllerModel : Singleton<DigTrenchEntryControllerModel>
    {
        public StorageDigTrench DigTrench
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().DigTrench; }
        }

        public bool IsUnLock(DigTrenchLevel config)
        {
            if (config == null)
                return false;
            
            if (config.unlockNodeNum <= 0)
                return true;

            if (IsPlayLevel(config))
                return true;

            return DecoManager.Instance.GetOwnedNodeNum() >= config.unlockNodeNum;
        }

        public bool IsPlayLevel(DigTrenchLevel config)
        {
            if (config == null)
                return false;

            if (IsFinish(config))
                return true;

            return false;
        }
        
        public bool IsFinish(DigTrenchLevel config)
        {
            if (config == null)
                return false;

            return DigTrench.FinishInfo.ContainsKey(config.id);
        }

        public void FinishLevel(DigTrenchLevel config)
        {
            if(config == null)
                return;
            
            if(DigTrench.FinishInfo.ContainsKey(config.id))
                return;
            if (config.id == 99)
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFirstcomplete1);
                var bag = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find("Surface/Movie_Ship/ripple/SkeletonUtility-SkeletonRoot/root/chuan/bag");
                if (bag)
                    bag.gameObject.SetActive(false);
            }
            else if (config.id == 1)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFirstcomplete2);
            else if (config.id == 2)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFirstcomplete3);
            else if (config.id == 3)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFirstcomplete4);
            else if (config.id == 4)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFirstcomplete5);
            else if (config.id == 5)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFirstcomplete6);
            
            DigTrench.FinishInfo.Add(config.id, true);
        }

        public bool HaveNoFinishLevel()
        {
            var configs = ASMR.Model.Instance.GetMiniGameItem(UIPopupGameTabulationController.MiniGameTypeTab.DigTrench);
            if (configs == null)
                return false;
            
            foreach (var item in configs)
            {
                var DigTrenchLevel = DigTrenchConfigManager.Instance.DigTrenchLevelList.Find(a => a.id == item.configId);
                if(DigTrenchLevel == null)
                    continue;
                
                if (IsUnLock(DigTrenchLevel) && !IsFinish(DigTrenchLevel))
                    return true;
            }

            return false;
        }
        
        public bool IsShowHand()
        {
            for(int i = 0; i <  DigTrenchConfigManager.Instance.DigTrenchLevelList.Count; i++)
            {
                if (i == 3 && i == 4)
                {
                    if (IsUnLock(DigTrenchConfigManager.Instance.DigTrenchLevelList[i]) && !IsFinish(DigTrenchConfigManager.Instance.DigTrenchLevelList[i]))
                        return true;
                }
            }

            return false;
        }
    }
}