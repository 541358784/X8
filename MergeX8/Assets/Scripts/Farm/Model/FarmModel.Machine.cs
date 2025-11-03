using Deco.Node;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Farm.FarmFly;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void Production(DecoNode node, TableFarmMachineOrder config)
        {
            if(!storageFarm.Machine.ContainsKey(node.Id))
                storageFarm.Machine.Add(node.Id, new StorageMachine());

            storageFarm.Machine[node.Id].Id = node.Id;
            storageFarm.Machine[node.Id].OrderId = config.Id;
            storageFarm.Machine[node.Id].RipeningTime = (long)APIManager.Instance.GetServerTime()+config.RipeningTime*1000;
            storageFarm.Machine[node.Id].StartTime = (long)APIManager.Instance.GetServerTime();
            storageFarm.Machine[node.Id].CdTime = config.RipeningTime;
            
            Load(node);
            var machineConfig = FarmConfigManager.Instance.TableFarmMachineList.Find(a => a.DecoNode == node.Id);
            if(machineConfig != null)
                SendBI_FarmWork(BiWorkType.Generate, machineConfig.Id, config.Id);
        }

        private void LoadMachine(DecoNode node)
        {
            if(!node.IsOwned)
                return;
            
            if(node.CurrentItem == node.DefaultItem)
                return;
            
            CreateLogic(node._currentItem.GameObject.transform, node, FarmType.Machine);
        }

        private void UnLoadMachine(DecoNode node)
        {
            RemoveLogic(node, FarmType.Machine);
        }

        public FarmProductStatus GetMachineProductStatus(DecoNode node)
        {
            var type = FarmConfigManager.Instance.GetDecoNodeType(node.Id);
            if (type != FarmType.Machine)
                return FarmProductStatus.None;

            if (!storageFarm.Machine.ContainsKey(node.Id))
                return FarmProductStatus.Free;
            
            if((long)APIManager.Instance.GetServerTime() <= storageFarm.Machine[node.Id].RipeningTime)
                return FarmProductStatus.Producing;
            
            return FarmProductStatus.Finish;
        }

        private bool HaveFinishMachine()
        {
            long serverTime = (long)APIManager.Instance.GetServerTime();
            foreach (var kv in storageFarm.Machine)
            {
                if (serverTime > kv.Value.RipeningTime)
                    return true;
            }

            return false;
        }
        
        public bool GainMachineProduct(DecoNode node)
        {
            if (GetMachineProductStatus(node) != FarmProductStatus.Finish)
                return false;

            var config = FarmConfigManager.Instance.GetFarmMachineOrderConfig(storageFarm.Machine[node.Id].OrderId);
            if (config == null)
                return false;

            storageFarm.Machine.Remove(node.Id);
            
            AddProductItem(config.ProductItem, config.ProductNum);
            UpdateStatus(node, FarmType.Machine);
            
            var machineConfig = FarmConfigManager.Instance.TableFarmMachineList.Find(a => a.DecoNode == node.Id);
            if(machineConfig != null)
                SendBI_FarmWork(BiWorkType.GainProduct, machineConfig.Id, config.Id);

            FarmFlyManager.Instance.FlyProductItemToWarehouse(node, config.ProductItem, config.ProductNum);
            return true;
        }

        public StorageMachine GetStorageMachine(DecoNode node)
        {
            if (!storageFarm.Machine.ContainsKey(node.Id))
                return null;

            return storageFarm.Machine[node.Id];
        }
        
        public void SpeedMachine(DecoNode node, int time)
        {
            var storage = GetStorageMachine(node);
            if (storage == null)
                return;

            storage.RipeningTime -= time;
        }
    }
}