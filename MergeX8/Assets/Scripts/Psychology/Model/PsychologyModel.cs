using System.Collections.Generic;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace Psychology.Model
{
    public class PsychologyModel : Singleton<PsychologyModel>
    {
        public int debugLevelId { get; set; }

        public TablePsychology _config;
        
        public StorageConnectLine Psychology
        {
            get { return StorageManager.Instance.GetStorage<StorageHome>().Psychology; }
        }
        
        public void InitModel(TablePsychology config)
        {
            _config = config;
        }
        
     

        public bool IsUnLock(TablePsychology config)
        {
            if (config == null)
                return false;
            
            if (config.unlockNodeNum <= 0)
                return true;

            if (IsPlayLevel(config))
                return true;

            return DecoManager.Instance.GetOwnedNodeNum() >= config.unlockNodeNum;
        }

        public bool IsPlayLevel(TablePsychology config)
        {
            if (config == null)
                return false;

            if (IsFinish(config))
                return true;

            return false;
        }
        
        public bool IsFinish(TablePsychology config)
        {
            if (config == null)
                return false;
            
            return IsFinish(config.id);
        }

        public bool IsFinish(int id)
        {
            return Psychology.FinishInfo.ContainsKey(id);
        }
        
        public void FinishLevel(int levelId)
        {
            if(Psychology.FinishInfo.ContainsKey(levelId))
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
            
            Psychology.FinishInfo.Add(levelId, true);
        }

        public bool HaveNoFinishLevel()
        {
            foreach (var TableOnePathLevel in PsychologyConfigManager.Instance._configs)
            {
                if (IsUnLock(TableOnePathLevel) && !IsFinish(TableOnePathLevel))
                    return true;
            }

            return false;
        }
        
        public bool IsShowHand()
        {
            for(int i = 0; i <  PsychologyConfigManager.Instance._configs.Count; i++)
            {
                if (i == 3 && i == 4)
                {
                    if (IsUnLock(PsychologyConfigManager.Instance._configs[i]) && !IsFinish(PsychologyConfigManager.Instance._configs[i]))
                        return true;
                }
            }

            return false;
        }
        
        
        public Dictionary<string, string> GetBaseDownLoadAssets()
        {
            List<AssetGroup> resGroupList = GetBaseDownloadAssetGroups();
            if (resGroupList == null) 
                return new Dictionary<string, string>();

            return AssetCheckManager.Instance.ResDownloadFitter(resGroupList);
        }
        
        public List<AssetGroup> GetBaseDownloadAssetGroups()
        {
            var assets = new List<AssetGroup>();
            
            var variantPostFix = ResourcesManager.Instance.IsSdVariant ? "sd" : "hd";
            
            assets.Add(new AssetGroup("Psychology", $"SpriteAtlas/PsychologyAtlas/{variantPostFix}.ab"));
            assets.Add(new AssetGroup("Psychology", "prefabs/Psychology.ab"));
            
            return assets;
        }
    }
}