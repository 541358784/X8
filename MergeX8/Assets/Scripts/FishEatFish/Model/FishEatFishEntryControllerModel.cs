using Decoration;
using Decoration.DaysManager;
using DragonPlus;
using DragonPlus.Config.FishEatFish;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace FishEatFishSpace
{
    public class FishEatFishEntryControllerModel : Singleton<FishEatFishEntryControllerModel>
    {
        public StorageFishEatFish FishEatFish
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().FishEatFish; }
        }

        public bool IsUnLock(FishEatFishLevel config)
        {
            if (config == null)
                return false;
            
            if (config.unlockNodeNum <= 0)
                return true;

            if (IsPlayLevel(config))
                return true;

            return DecoManager.Instance.GetOwnedNodeNum() >= config.unlockNodeNum;
        }

        public bool IsPlayLevel(FishEatFishLevel config)
        {
            if (config == null)
                return false;

            if (IsFinish(config))
                return true;

            return false;
        }
        
        public bool IsFinish(FishEatFishLevel config)
        {
            if (config == null)
                return false;

            return FishEatFish.FinishInfo.ContainsKey(config.id);
        }

        public void FinishLevel(FishEatFishLevel config)
        {
            if(config == null)
                return;
            
            if(FishEatFish.FinishInfo.ContainsKey(config.id))
                return;
            if (config.id == 1)
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishgameFirstcomplete1);
                var bag = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find("Surface/Movie_Ship/ripple/SkeletonUtility-SkeletonRoot/root/chuan/bag");
                if (bag)
                    bag.gameObject.SetActive(false);
            }
            else if (config.id == 2)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishgameFirstcomplete2);
            else if (config.id == 3)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishgameFirstcomplete3);
            else if (config.id == 4)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishgameFirstcomplete4);
            else if (config.id == 5)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventFishgameFirstcomplete5);
            
            FishEatFish.FinishInfo.Add(config.id, true);
        }

        public bool HaveNoFinishLevel()
        {
            var configs = ASMR.Model.Instance.GetMiniGameItem(UIPopupGameTabulationController.MiniGameTypeTab.FishEatFish);
            if (configs == null)
                return false;
            
            foreach (var item in configs)
            {
                var FishEatFishLevel = FishEatFishConfigManager.Instance.FishEatFishLevelList.Find(a => a.id == item.configId);
                if(FishEatFishLevel == null)
                    continue;
                
                if (IsUnLock(FishEatFishLevel) && !IsFinish(FishEatFishLevel))
                    return true;
            }

            return false;
        }
        
        public bool IsShowHand()
        {
            for(int i = 0; i <  FishEatFishConfigManager.Instance.FishEatFishLevelList.Count; i++)
            {
                if (i == 3 && i == 4)
                {
                    if (IsUnLock(FishEatFishConfigManager.Instance.FishEatFishLevelList[i]) && !IsFinish(FishEatFishConfigManager.Instance.FishEatFishLevelList[i]))
                        return true;
                }
            }

            return false;
        }
    }
}