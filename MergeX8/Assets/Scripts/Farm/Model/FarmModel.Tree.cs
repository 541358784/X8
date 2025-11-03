using Deco.Node;
using Decoration;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Farm.FarmFly;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void GrowTree(DecoNode node)
        {
            var config = FarmConfigManager.Instance.GetFarmTreeConfig(node.Id);
            if(config == null)
                return;
            
            if(!storageFarm.Tree.ContainsKey(node.Id))
                storageFarm.Tree.Add(node.Id, new StorageTree());

            storageFarm.Tree[node.Id].Id = node.Id;
            storageFarm.Tree[node.Id].RipeningTime = (long)APIManager.Instance.GetServerTime()+config.RipeningTime*1000;
            storageFarm.Tree[node.Id].StartTime = (long)APIManager.Instance.GetServerTime();
            storageFarm.Tree[node.Id].CdTime = config.RipeningTime;
            
            SendBI_FarmWork(BiWorkType.Generate, config.Id, config.ProductItem);
            
            Load(node);
        }
        
        private void LoadTree(DecoNode node)
        {
            if(node._currentItem == null)
                return;
            
            //树特殊处理 第一次 立马可领取
            if (!storageFarm.Tree.ContainsKey(node.Id))
            {
                GrowTree(node);
                storageFarm.Tree[node.Id].RipeningTime = 0;
                storageFarm.Tree[node.Id].StartTime = 0;
            }
            
            CreateLogic(node._currentItem.GameObject.transform, node, FarmType.Tree);
        }

        private void UnLoadTree(DecoNode node)
        {
            RemoveLogic(node, FarmType.Tree);
        }
        
        public FarmProductStatus GetTreeProductStatus(DecoNode node)
        {
            var type = FarmConfigManager.Instance.GetDecoNodeType(node.Id);
            if (type != FarmType.Tree)
                return FarmProductStatus.None;

            if (!storageFarm.Tree.ContainsKey(node.Id))
                return FarmProductStatus.Free;
            
            if((long)APIManager.Instance.GetServerTime() <= storageFarm.Tree[node.Id].RipeningTime)
                return FarmProductStatus.Producing;
            
            return FarmProductStatus.Finish;
        }

        private bool HaveFinishTree()
        {
            long serverTime = (long)APIManager.Instance.GetServerTime();
            foreach (var kv in storageFarm.Tree)
            {
                var node = DecoManager.Instance.FindNode(kv.Value.Id);
                if(node == null)
                    continue;
                
                if(!Instance.IsUnLockNode(node))
                    continue;
                    
                if (serverTime > kv.Value.RipeningTime)
                    return true;
            }

            return false;
        }
        
        public bool GainTreeProduct(DecoNode node)
        {
            if (GetTreeProductStatus(node) != FarmProductStatus.Finish)
                return false;

            var config = FarmConfigManager.Instance.GetFarmTreeConfig(node.Id);
            if (config == null)
                return false;

            storageFarm.Tree.Remove(node.Id);
            
            AddProductItem(config.ProductItem, config.ProductNum);
            GrowTree(node);
            UpdateStatus(node, FarmType.Tree);
            FarmFlyManager.Instance.FlyProductItemToWarehouse(node, config.ProductItem, config.ProductNum);
            SendBI_FarmWork(BiWorkType.GainProduct, config.Id, config.ProductItem);
            
            return true;
        }
        
        public StorageTree GetStorageTree(DecoNode node)
        {
            if (!storageFarm.Tree.ContainsKey(node.Id))
                return null;

            return storageFarm.Tree[node.Id];
        }
        
        public void SpeedTree(DecoNode node, int time)
        {
            var storage = GetStorageTree(node);
            if (storage == null)
                return;

            storage.RipeningTime -= time;
        }
    }
}