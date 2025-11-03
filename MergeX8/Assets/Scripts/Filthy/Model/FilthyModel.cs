using System.Collections.Generic;
using Decoration;
using DragonPlus.Config.Filthy;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Filthy.Game;

namespace Filthy.Model
{
    public partial class FilthyModel : Manager<FilthyModel>
    {
        public enum NodeState
        {
            Lock = 0,
            UnLock = 1,
            Owned = 2,
            Finish = 3
        }

        public FilthySetting _config;

        public StorageFilthy StorageFilthy
        {
            get { return StorageManager.Instance.GetStorage<StorageMiniGames>().Filthy; }
        }

        public int CurrentLevelID()
        {
            if (StorageFilthy.CurrentLevel == 0)
                StorageFilthy.CurrentLevel = 1;
            
            return StorageFilthy.CurrentLevel;
        }
        
        public int LevelId()
        {
            return _config.LevelId;
        }

        public int ResLevelId()
        {
            return _config.ResLevel;
        }
        public void InitLevel(int levelId)
        {
            _config = FilthyConfigManager.Instance.GetSettingConfig().Find(a => a.Id == levelId);

            GuideSubSystem.Instance.SetFilterGuide(_config.GuideIds);
            
            InitStorage(_config.LevelId);
            FilthyGameLogic.Instance.LoadLevel();
        }

        public bool IsFinish(int levelId)
        {
            return StorageFilthy.FinishInfo.ContainsKey(levelId);
        }

        public void Release()
        {
            FilthyGameLogic.Instance.Release();
            GuideSubSystem.Instance.CleanFilterGuide();
        }

        private void InitStorage(int levelId)
        {
            if (StorageFilthy.Levels.ContainsKey(levelId))
                return;
            
            StorageFilthy.Levels.Add(levelId, new StorageFilthyLevel());

            var nodes = FilthyConfigManager.Instance.FilthyNodesList.FindAll(a => a.LevelId == levelId);
            foreach (var node in nodes)
            {
                if (StorageFilthy.Levels[levelId].Nodes.ContainsKey(node.Id))
                    continue;

                StorageFilthy.Levels[levelId].Nodes.Add(node.Id, new StorageFilthyNode());
                StorageFilthy.Levels[levelId].Nodes[node.Id].Id = node.Id;
                StorageFilthy.Levels[levelId].Nodes[node.Id].State = node.DefatultOpen ? (int)NodeState.UnLock : (int)NodeState.Lock;
            }
        }

        public NodeState GetNodeState(int levelId, int nodeId)
        {
            var item = GetNode(levelId, nodeId);
            if (item == null)
                return NodeState.Lock;

            return (NodeState)item.State;
        }

        public StorageFilthyNode GetNode(int levelId, int nodeId)
        {
            if (!StorageFilthy.Levels.ContainsKey(levelId))
                return null;

            if (!StorageFilthy.Levels[levelId].Nodes.ContainsKey(nodeId))
                return null;

            return StorageFilthy.Levels[levelId].Nodes[nodeId];
        }

        public void OwnedNode(int levelId, int nodeId)
        {
            var item = GetNode(levelId, nodeId);
            if (item == null)
                return;

            item.State = item.State <= (int)NodeState.Owned ? (int)NodeState.Owned : item.State;

            UnLockNextNode(nodeId);
        }

        public void FinishNode(int levelId, int nodeId)
        {
            var item = GetNode(levelId, nodeId);
            if (item == null)
                return;

            item.State = (int)NodeState.Finish;
            foreach (var node in StorageFilthy.Levels[levelId].Nodes)
            {
                if (node.Value.State == (int)NodeState.Finish)
                    continue;

                return;
            }

            FinishLevel(levelId);
        }

        public void FinishLevel(int levelId)
        {
            if (StorageFilthy.FinishInfo.ContainsKey(levelId))
                return;

            StorageFilthy.FinishInfo.Add(levelId, true);

            int index = FilthyConfigManager.Instance.GetSettingConfig().FindIndex(a => a.LevelId == levelId);
            if(index < 0)
                return;
            
            if(index+1 >= FilthyConfigManager.Instance.GetSettingConfig().Count)
                return;

            StorageFilthy.CurrentLevel = FilthyConfigManager.Instance.GetSettingConfig()[index + 1].Id;
        }

        public void UnLockNextNode(int nodeId)
        {
            var nodeConfig = FilthyConfigManager.Instance.FilthyNodesList.Find(a => a.Id == nodeId);
            if (nodeConfig == null)
                return;

            if (nodeConfig.NextNodeId <= 0)
                return;

            UnlockNode(nodeConfig.LevelId, nodeConfig.NextNodeId);
        }

        public void UnlockNode(int levelId, int nodeId)
        {
            var item = GetNode(levelId, nodeId);
            if (item == null)
                return;

            item.State = item.State <= (int)NodeState.Lock ? (int)NodeState.UnLock : item.State;
        }

        public Dictionary<string, string> GetBaseDownLoadAssets(int levelId)
        {
            List<AssetGroup> resGroupList = GetBaseDownloadAssetGroups(levelId);
            if (resGroupList == null)
                return new Dictionary<string, string>();

            return AssetCheckManager.Instance.ResDownloadFitter(resGroupList);
        }

        public List<AssetGroup> GetBaseDownloadAssetGroups(int levelId)
        {
            var assets = new List<AssetGroup>();

            var variantPostFix = ResourcesManager.Instance.IsSdVariant ? "sd" : "hd";

            assets.Add(new AssetGroup("Filthy", $"SpriteAtlas/StimulateAtlas/{variantPostFix}.ab"));
            assets.Add(new AssetGroup("Filthy", $"Stimulate/Levels/Level{levelId}/Prefabs.ab"));
            assets.Add(new AssetGroup("Filthy", $"Stimulate/Levels/Level{levelId}/Audio.ab"));
            assets.Add(new AssetGroup("Filthy", $"Stimulate/Levels/Level{levelId}/SpriteAtlas/Level{levelId}/{variantPostFix}.ab"));
            assets.Add(new AssetGroup("Filthy", "prefabs/stimulate.ab"));

            return assets;
        }
    }
}