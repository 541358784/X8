using DragonPlus.Config.Farm;
using Gameplay;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public int WarehouseNum()
        {
            if (storageFarm.WarehouseId == 0)
            {
                storageFarm.WarehouseNum = FarmConfigManager.Instance.TableFarmWarehouseList[0].OpenNum;
                storageFarm.WarehouseId = FarmConfigManager.Instance.TableFarmWarehouseList[0].Id;
            }

            return storageFarm.WarehouseNum;
        }

        public bool IsWarehouseEnough(int num)
        {
            return WarehouseNum() - WarehouseOccupyNum() >= num;
        }

        public int WarehouseOccupyNum()
        {
            int num = 0;
            foreach (var kv in storageFarm.ProductItems)
            {
                if(UserData.Instance.IsFarmProp(kv.Key))
                    continue;
                
                num += kv.Value;
            }

            return num;
        }

        public void ExpansionWarehouse(int id)
        {
            var config = FarmConfigManager.Instance.TableFarmWarehouseList.Find(a => a.Id == id);
            if(config == null)
                return;
            
            storageFarm.WarehouseNum = config.OpenNum;
            storageFarm.WarehouseId += 1;
        }

        public bool IsExpansionWarehouseFill()
        {
            var index = FarmConfigManager.Instance.TableFarmWarehouseList.FindIndex(a => a.Id == storageFarm.WarehouseId);
            if(index < 0)
                return true;

            return index >= FarmConfigManager.Instance.TableFarmWarehouseList.Count-1;
        }

        public TableFarmWarehouse GetExpansionWarehouseConfig()
        {
            return FarmConfigManager.Instance.TableFarmWarehouseList.Find(a => a.Id == storageFarm.WarehouseId+1);
        }
    }
}