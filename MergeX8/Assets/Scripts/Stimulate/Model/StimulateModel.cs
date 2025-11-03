using System.Collections.Generic;
using Decoration;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Stimulate.Configs;

namespace Stimulate.Model
{
    public class StimulateModel : Manager<StimulateModel>
    {
        public enum NodeState
        {
            Lock = 0,
            UnLock = 1,
            Owned = 2,
            Finish = 3
        }
        
        public TableStimulateSetting _config;

        public StorageStimulate StorageStimulate
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>().Stimulate;
            }
        }
        
        public void InitLevel(TableStimulateSetting config)
        {
            _config = config;

            InitStorage();
            StimulateGameLogic.Instance.LoadLevel();
        }
        
        public bool IsFinish(TableStimulateSetting config)
        {
            return StorageStimulate.FinishInfo.ContainsKey(config.levelId);
        }
        
        public void Release()
        {
            StimulateGameLogic.Instance.Release();
        }

        private void InitStorage()
        {
            if(StorageStimulate.Levels.ContainsKey(_config.levelId))
                return;
            
            StorageStimulate.Levels.Add(_config.levelId, new StorageStimulateLevel());

            var nodes = StimulateConfigManager.Instance.GetNodes(_config.levelId);
            foreach (var node in nodes)
            {
                if(StorageStimulate.Levels[_config.levelId].Nodes.ContainsKey(node.id))
                    continue;

                StorageStimulate.Levels[_config.levelId].Nodes.Add(node.id, new StorageStimulateNode());
                StorageStimulate.Levels[_config.levelId].Nodes[node.id].Id = node.id;
                StorageStimulate.Levels[_config.levelId].Nodes[node.id].State = node.defatultOpen ? (int)NodeState.UnLock : (int)NodeState.Lock;
            }
        }

        public NodeState GetNodeState(int levelId, int nodeId)
        {
            var item = GetNode(levelId, nodeId);
            if(item == null)
                return NodeState.Lock;

            return (NodeState)item.State;
        }

        public StorageStimulateNode GetNode(int levelId, int nodeId)
        {
            if(!StorageStimulate.Levels.ContainsKey(levelId))
                return null;
            
            if(!StorageStimulate.Levels[_config.levelId].Nodes.ContainsKey(nodeId))
                return null;

            return StorageStimulate.Levels[_config.levelId].Nodes[nodeId];
        }

        public void OwnedNode(int levelId, int nodeId)
        {
            var item = GetNode(levelId, nodeId);
            if(item == null)
                return;

            item.State = item.State <= (int)NodeState.Owned ? (int)NodeState.Owned : item.State;
            
            UnLockNextNode(nodeId);
        }

        public void FinishNode(int levelId, int nodeId)
        {
            var item = GetNode(levelId, nodeId);
            if(item == null)
                return;

            item.State = (int)NodeState.Finish;
            foreach (var node in StorageStimulate.Levels[levelId].Nodes)
            {
                if(node.Value.State == (int)NodeState.Finish)
                    continue;
                
                return;
            }

            FinishLevel(levelId);
        }

        public void FinishLevel(int levelId)
        {
            if(StorageStimulate.FinishInfo.ContainsKey(levelId))
                return;
            
            StorageStimulate.FinishInfo.Add(levelId, true);
        }

        public void UnLockNextNode(int nodeId)
        {
            var nodeConfig = StimulateConfigManager.Instance._stimulateNodes.Find(a => a.id == nodeId);
            if(nodeConfig == null)
                return;
            
            if(nodeConfig.nextNodeId <= 0)
                return;

            UnlockNode(nodeConfig.levelId, nodeConfig.nextNodeId);
        }

        public void UnlockNode(int levelId, int nodeId)
        {
            var item = GetNode(levelId, nodeId);
            if(item == null)
                return;
            
            item.State = item.State <= (int)NodeState.Lock ? (int)NodeState.UnLock : item.State;
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
            
            assets.Add(new AssetGroup("Stimulate", $"SpriteAtlas/StimulateAtlas/{variantPostFix}.ab"));
            assets.Add(new AssetGroup("Stimulate", $"Stimulate/Levels/Level101/Prefabs.ab"));
            assets.Add(new AssetGroup("Stimulate", $"Stimulate/Levels/Level101/Audio.ab"));
            assets.Add(new AssetGroup("Stimulate", $"Stimulate/Levels/Level101/SpriteAtlas/Level101/{variantPostFix}.ab"));
            assets.Add(new AssetGroup("Stimulate", "prefabs/stimulate.ab"));
            
            
            return assets;
        }
    }
}