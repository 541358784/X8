using Decoration;
using Decoration.DaysManager;
using DragonPlus;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using OnePath;
using UnityEngine;

namespace OnePathSpace
{
    public class OnePathEntryControllerModel : Singleton<OnePathEntryControllerModel>
    {
        public StorageOnePath OnePath
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().OnePath; }
        }

        public bool IsUnLock(TableOnePathLevel config)
        {
            if (config == null)
                return false;
            
            if (config.unlockNodeNum <= 0)
                return true;

            if (IsPlayLevel(config))
                return true;

            return DecoManager.Instance.GetOwnedNodeNum() >= config.unlockNodeNum;
        }

        public bool IsPlayLevel(TableOnePathLevel config)
        {
            if (config == null)
                return false;

            if (IsFinish(config))
                return true;

            return false;
        }
        
        public bool IsFinish(TableOnePathLevel config)
        {
            if (config == null)
                return false;

            return OnePath.FinishInfo.ContainsKey(config.id);
        }

        public void FinishLevel(int levelId)
        {
            if(OnePath.FinishInfo.ContainsKey(levelId))
                return;
            if (levelId == 1)
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventOnePathFirstcomplete1);
                var bag = DecoManager.Instance.CurrentWorld.PinchMap.transform.Find("Surface/Movie_Ship/ripple/SkeletonUtility-SkeletonRoot/root/chuan/bag");
                if (bag)
                    bag.gameObject.SetActive(false);
            }
            else if (levelId == 2)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventOnePathFirstcomplete2);
            else if (levelId == 3)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventOnePathFirstcomplete3);
            else if (levelId == 4)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventOnePathFirstcomplete4);
            else if (levelId == 5)
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventOnePathFirstcomplete5);
            
            OnePath.FinishInfo.Add(levelId, true);
        }

        public bool HaveNoFinishLevel()
        {
            var configs = ASMR.Model.Instance.GetMiniGameItem(UIPopupGameTabulationController.MiniGameTypeTab.OnePath);
            if (configs == null)
                return false;
            
            foreach (var item in configs)
            {
                var TableOnePathLevel = OnePathConfigManager.Instance._configs.Find(a => a.id == item.configId);
                if(TableOnePathLevel == null)
                    continue;
                
                if (IsUnLock(TableOnePathLevel) && !IsFinish(TableOnePathLevel))
                    return true;
            }

            return false;
        }
        
        public bool IsShowHand()
        {
            for(int i = 0; i <  OnePathConfigManager.Instance._configs.Count; i++)
            {
                if (i == 3 && i == 4)
                {
                    if (IsUnLock(OnePathConfigManager.Instance._configs[i]) && !IsFinish(OnePathConfigManager.Instance._configs[i]))
                        return true;
                }
            }

            return false;
        }
    }
}