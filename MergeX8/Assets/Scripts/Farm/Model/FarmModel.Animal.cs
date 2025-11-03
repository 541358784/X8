using System.Collections.Generic;
using Deco.Node;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Farm.FarmFly;
using Farm.Order;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void FeedAnim(DecoNode node, TableFarmAnimal config)
        {
            if(!storageFarm.Animal.ContainsKey(node.Id))
                storageFarm.Animal.Add(node.Id, new StorageAnimal());

            storageFarm.Animal[node.Id].Id = node.Id;
            storageFarm.Animal[node.Id].RipeningTime = (long)APIManager.Instance.GetServerTime()+config.RipeningTime*1000;
            storageFarm.Animal[node.Id].StartTime = (long)APIManager.Instance.GetServerTime();
            storageFarm.Animal[node.Id].CdTime = config.RipeningTime;

            SendBI_FarmWork(BiWorkType.Generate, config.Id, config.ProductItem);
            Load(node);
        }
        private void LoadAnimal(DecoNode node)
        {
            if(!node.IsOwned)
                return;
            
            if(node.CurrentItem == node.DefaultItem)
                return;
            
            CreateLogic(node._currentItem.GameObject.transform, node, FarmType.Animal);
        }

        private void UnLoadAnimal(DecoNode node)
        {
            RemoveLogic(node, FarmType.Animal);
        }

        private bool HaveFinishAnimal()
        {
            long serverTime = (long)APIManager.Instance.GetServerTime();
            foreach (var kv in storageFarm.Animal)
            {
                if (serverTime > kv.Value.RipeningTime)
                    return true;
            }

            return false;
        }
        
        public FarmProductStatus GetAnimalProductStatus(DecoNode node)
        {
            var type = FarmConfigManager.Instance.GetDecoNodeType(node.Id);
            if (type != FarmType.Animal)
                return FarmProductStatus.None;

            if (!storageFarm.Animal.ContainsKey(node.Id))
                return FarmProductStatus.Free;
            
            if((long)APIManager.Instance.GetServerTime() <= storageFarm.Animal[node.Id].RipeningTime)
                return FarmProductStatus.Producing;
            
            return FarmProductStatus.Finish;
        }

        public bool GainAnimalProduct(DecoNode node)
        {
            if (GetAnimalProductStatus(node) != FarmProductStatus.Finish)
                return false;

            var config = FarmConfigManager.Instance.GetFarmAnimConfig(node.Id);
            if (config == null)
                return false;

            storageFarm.Animal.Remove(node.Id);
            
            AddProductItem(config.ProductItem, config.ProductNum);
            UpdateStatus(node, FarmType.Animal);
            SendBI_FarmWork(BiWorkType.GainProduct, config.Id, config.ProductItem);
           
            FarmFlyManager.Instance.FlyProductItemToWarehouse(node, config.ProductItem, config.ProductNum);
            return true;
        }
        
        public StorageAnimal GetStorageAnimal(DecoNode node)
        {
            if (!storageFarm.Animal.ContainsKey(node.Id))
                return null;

            return storageFarm.Animal[node.Id];
        }
        
        public void SpeedAnimal(DecoNode node, int time)
        {
            var storage = GetStorageAnimal(node);
            if (storage == null)
                return;

            storage.RipeningTime -= time;
        }
    }
}