using System.Collections.Generic;
using System.Linq;
using Farm.Model;

namespace DragonPlus.Config.Farm
{
    public partial class FarmConfigManager
    {
        private Dictionary<int, FarmType> _linkDecoNode = new Dictionary<int, FarmType>();
        private Dictionary<SeedType, List<TableFarmSeed>> _farmSeedMap = new Dictionary<SeedType, List<TableFarmSeed>>();
        private Dictionary<int, List<TableFarmOrderItem>> _farmOrderItemMap = new Dictionary<int, List<TableFarmOrderItem>>();
        
        protected override void Trim()
        {
            _linkDecoNode.Clear();
            
            TableFarmTreeList.ForEach(a=>_linkDecoNode.Add(a.DecoNode, FarmType.Tree));
            TableFarmAnimalList.ForEach(a=>_linkDecoNode.Add(a.DecoNode, FarmType.Animal));
            TableFarmMachineList.ForEach(a=>_linkDecoNode.Add(a.DecoNode,FarmType.Machine));
            TableFarmGroundList.ForEach(a=>_linkDecoNode.Add(a.DecoNode,FarmType.Ground));
            
            _farmSeedMap.Clear();
            foreach (var config in TableFarmSeedList)
            {
                var type = (SeedType)config.Type;
                if(!_farmSeedMap.ContainsKey(type))
                    _farmSeedMap.Add(type, new List<TableFarmSeed>());
                    
                _farmSeedMap[type].Add(config);
            }
            
            _farmOrderItemMap.Clear();
        }

        public bool IsLinkDecoNode(int id)
        {
            return _linkDecoNode.ContainsKey(id);
        }

        public FarmType GetDecoNodeType(int id)
        {
            if (!IsLinkDecoNode(id))
                return FarmType.None;
            
            return _linkDecoNode[id];
        }

        public FarmType GetItemType(int id)
        {
            if (TableFarmSeedList.Find(a => a.Id == id) != null)
                return FarmType.Ground;

            if (TableFarmSeedList.Find(a => a.ProductItem == id) != null)
                return FarmType.Ground;
            
            if (TableFarmAnimalList.Find(a => a.ProductItem == id) != null)
                return FarmType.Animal;

            if (TableFarmTreeList.Find(a => a.ProductItem == id) != null)
                return FarmType.Tree;
                    
            if (TableFarmMachineOrderList.Find(a => a.ProductItem == id) != null)
                return FarmType.Machine;

            return FarmType.None;
        }
        
        public List<TableFarmSeed> GetFarmSeeds(int type)
        {
            return GetFarmSeeds((SeedType)type);
        }
        
        public TableFarmSeed GetFarmSeed(int id)
        {
            return TableFarmSeedList.Find(a => a.Id == id);
        }
        
        public List<TableFarmSeed> GetFarmSeeds(SeedType type)
        {
            if (!_farmSeedMap.ContainsKey(type))
                return null;

            return _farmSeedMap[type];
        }
        
        public int GetLinkDecoNodeUnLockLevel(int id)
        {
            if (!IsLinkDecoNode(id))
                return -1;

            var type = _linkDecoNode[id];
            switch (type)
            {
                case FarmType.Tree:
                {
                    return TableFarmTreeList.Find(a => a.DecoNode == id).UnlockLevel;
                }
                case FarmType.Animal:
                {
                    return TableFarmAnimalList.Find(a => a.DecoNode == id).UnlockLevel;
                }
                case FarmType.Machine:
                {
                    return TableFarmMachineList.Find(a => a.DecoNode == id).UnlockLevel;
                }
                case FarmType.Ground:
                {
                    return TableFarmGroundList.Find(a => a.DecoNode == id).UnlockLevel;
                }
            }

            return -1;
        }

        public TableFarmGround GetFarmGroundConfig(int nodeId)
        {
            return TableFarmGroundList.Find(a => a.DecoNode == nodeId);
        }

        public TableFarmMachine GetFarmMachineConfig(int nodeId)
        {
            return TableFarmMachineList.Find(a => a.DecoNode == nodeId);
        }

        public TableFarmMachineOrder GetFarmMachineOrderConfig(int id)
        {
            return TableFarmMachineOrderList.Find(a => a.Id == id);
        }
        
        public TableFarmAnimal GetFarmAnimConfig(int nodeId)
        {
            return TableFarmAnimalList.Find(a => a.DecoNode == nodeId);
        }
        
        public TableFarmTree GetFarmTreeConfig(int nodeId)
        {
            return TableFarmTreeList.Find(a => a.DecoNode == nodeId);
        }
        
        public int GetFarmProductPrice(int id)
        {
            var config = TableFarmProductList.Find(a => a.Id == id);
            if (config == null)
                return 0;

            return config.Award;
        }

        public TableFarmProduct GetFarmProductConfig(int id)
        {
           return TableFarmProductList.Find(a => a.Id == id);
        }
        
        public string GetFarmProductIcon(int id)
        {
            var config = GetFarmProductConfig(id);
            if (config == null)
                return "";

            return config.Image;
        }
        
        public List<TableFarmOrderItem> GetFarmOrderItemByLevel(int level)
        {
            if (!_farmOrderItemMap.ContainsKey(level))
            {
                _farmOrderItemMap.Add(level, new List<TableFarmOrderItem>());

                for (int i = TableFarmOrderItemList.Count - 1; i >= 0; i--)
                {
                    var config = TableFarmOrderItemList[i];
                    if(config.Level > level)
                        continue;
                
                    if(_farmOrderItemMap[level].Find(a=>a.Id == config.Id) != null)
                        continue;
                    
                    _farmOrderItemMap[level].Add(config);
                }
            }
                
            return _farmOrderItemMap[level];
        }
    }
}