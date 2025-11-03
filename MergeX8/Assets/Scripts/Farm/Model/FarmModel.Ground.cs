using System.Collections.Generic;
using Deco.Node;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Farm.FarmFly;
using UnityEngine;

namespace Farm.Model
{
    public partial class FarmModel
    {
        private Dictionary<int, GameObject> _groundSeeds = new Dictionary<int, GameObject>();
        
        public void CultivateSeed(DecoNode node, TableFarmSeed config)
        {
            if(!storageFarm.Ground.ContainsKey(node.Id))
                storageFarm.Ground.Add(node.Id, new StorageGround());

            storageFarm.Ground[node.Id].Id = node.Id;
            storageFarm.Ground[node.Id].SeedId = config.Id;
            storageFarm.Ground[node.Id].RipeningTime = (long)APIManager.Instance.GetServerTime()+config.RipeningTime*1000;
            storageFarm.Ground[node.Id].StartTime = (long)APIManager.Instance.GetServerTime();
            storageFarm.Ground[node.Id].CdTime = config.RipeningTime;
            
            var groundConfig = FarmConfigManager.Instance.TableFarmGroundList.Find(a => a.DecoNode == node.Id);
            if(groundConfig != null)
                SendBI_FarmWork(BiWorkType.Generate, groundConfig.Id, config.Id);
            Load(node);
        }

        public bool IsCultivate(DecoNode node)
        {
            return storageFarm.Ground.ContainsKey(node.Id);
        }

        public StorageGround GetStorageGround(DecoNode node)
        {
            if (!IsCultivate(node))
                return null;

            return storageFarm.Ground[node.Id];
        }
        
        private void LoadSeed(DecoNode node)
        {
            if(!storageFarm.Ground.ContainsKey(node.Id))
                return;
            
            if(_groundSeeds.ContainsKey(node.Id))
                return;

            var config = FarmConfigManager.Instance.TableFarmSeedList.Find(a => a.Id == storageFarm.Ground[node.Id].SeedId);
            if(config == null)
                return;
            
            var prefab = _poolManager.SpawnGameObject($"Farm/Prefabs/Seeds/{config.Prefab}");
            if(prefab == null)
                return;

            prefab.transform.SetParent(node.IconTipTransform == null ? node.GameObject.transform : node.IconTipTransform);
            prefab.transform.localPosition = Vector3.zero;
            prefab.transform.localScale = Vector3.one;
            
            CreateLogic(prefab.transform, node, FarmType.Ground);
            
            _groundSeeds.Add(node.Id, prefab);
        }

        private void UnLoadSeed(DecoNode node)
        {
            if(!_groundSeeds.ContainsKey(node.Id))
                return;

            RemoveLogic(node, FarmType.Ground);
                
            _poolManager.RecycleGameObject(_groundSeeds[node.Id]);
            _groundSeeds.Remove(node.Id);
        }
        
        public FarmProductStatus GetGroundProductStatus(DecoNode node)
        {
            var type = FarmConfigManager.Instance.GetDecoNodeType(node.Id);
            if (type != FarmType.Ground)
                return FarmProductStatus.None;

            if (!storageFarm.Ground.ContainsKey(node.Id))
                return FarmProductStatus.Free;
            
            if((long)APIManager.Instance.GetServerTime() <= storageFarm.Ground[node.Id].RipeningTime)
                return FarmProductStatus.Producing;
            
            return FarmProductStatus.Finish;
        }

        private bool HaveFinishGround()
        {
            long serverTime = (long)APIManager.Instance.GetServerTime();
            foreach (var kv in storageFarm.Ground)
            {
                if (serverTime > kv.Value.RipeningTime)
                    return true;
            }

            return false;
        }
        
        public bool GainGroundProduct(DecoNode node)
        {
            if (GetGroundProductStatus(node) != FarmProductStatus.Finish)
                return false;

            var config = FarmConfigManager.Instance.GetFarmSeed(storageFarm.Ground[node.Id].SeedId);
            if (config == null)
                return false;

            storageFarm.Ground.Remove(node.Id);
            
            AddProductItem(config.ProductItem, config.ProductNum);
            UnLoadSeed(node);
            FarmFlyManager.Instance.FlyProductItemToWarehouse(node, config.ProductItem, config.ProductNum);
            
            var groundConfig = FarmConfigManager.Instance.TableFarmGroundList.Find(a => a.DecoNode == node.Id);
            if(groundConfig != null)
                SendBI_FarmWork(BiWorkType.GainProduct, groundConfig.Id, config.Id);
            
            UpdateBubble(node);
            
            return true;
        }

        public void SpeedSeed(DecoNode node, int time)
        {
            var storage = GetStorageGround(node);
            if (storage == null)
                return;

            storage.RipeningTime -= time;
        }
    }
}